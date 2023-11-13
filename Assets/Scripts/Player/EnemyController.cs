using Assets.Scripts.Player;
using LewdieJam.Game;
using LewdieJam.Map;
using LewdieJam.SO;
using LewdieJam.VN;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace LewdieJam.Player
{
    public class EnemyController : ACharacter
    {
        private ACharacter _target;

        [SerializeField]
        private GameObject _hintCircle;

        [SerializeField]
        private GameObject _charmedPrefab;

        private GameObject _attackTarget;

        private readonly List<ACharacter> _inRange = new();

        private GameObject _charmedEffect;

        private Animator _anim;

        // PROJECTILE MANAGEMENT

        /// <summary>
        /// If we throwed a projectile, are we waiting for it to collide with something
        /// </summary>
        public bool IsWaitingForProjectile { set; private get; }
        public PlayerController HookTarget { set; private get; }
        private float _pendingProjectileTimer;

        private bool _isCharmed;
        public bool IsCharmed
        {
            set
            {
                if (!_info.CanBeCharmed) return;

                _isCharmed = value;

                EnemyManager.Instance.RefreshAllTargets();

                if (_charmedEffect != null)
                {
                    Destroy(_charmedEffect);
                    _charmedEffect = null;
                }
                if (_isCharmed)
                {
                    _charmedEffect = Instantiate(_charmedPrefab, transform);
                }

                if (_attackTarget != null)
                {
                    Destroy(_attackTarget);
                    _attackTarget = null;
                }
            }
            get => _isCharmed;
        }

        public override Team Team => IsCharmed ? Team.Allie : Team.Enemy;

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);

            if (_info.IsBoss)
            {
                // TODO: Might cause issues if we have many boss
                GameManager.Instance.UpdateHealthBar(_health / (float)_info.BaseHealth);
            }

            if (damage > 0 && IsCharmed)
            {
                IsCharmed = false;
            }
        }

        public void RefreshTarget()
        {
            _target = _inRange
                .Where(x => x.Team != Team)
                .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
                .FirstOrDefault();
        }

        private void Awake()
        {
            AwakeParent();

            _anim = GetComponentInChildren<Animator>();

            // Ignore player if out of range
            var trigger = GetComponentInChildren<TriggerListener>();
            trigger.OnTriggerEnterCallback.AddListener((coll) =>
            {
                if (coll.CompareTag("Player") || coll.CompareTag("Enemy"))
                {
                    if (coll.CompareTag("Player") && _info.IsBoss)
                    {
                        GameManager.Instance.EnableBossHealthBar(_info.Name);
                    }

                    var other = coll.GetComponent<ACharacter>();
                    _inRange.Add(other);
                    if (other.Team != Team)
                    {
                        _target = other;
                    }
                }
            });
            trigger.OnTriggerExitCallback.AddListener((coll) =>
            {
                _inRange.RemoveAll(x => x.gameObject.GetInstanceID() == coll.gameObject.GetInstanceID());
                if (_target != null && (coll.CompareTag("Player") || coll.CompareTag("Enemy")) && coll.gameObject.GetInstanceID() == _target.gameObject.GetInstanceID())
                {
                    if (coll.CompareTag("Player") && _info.IsBoss)
                    {
                        GameManager.Instance.DisableBossHealthBar();
                    }
                    _target = null;
                }
            });
        }

        private void Start()
        {
            StartParent();
            EnemyManager.Instance.Register(this);
        }

        private void Update()
        {
            UpdateParent();

            // Update timer
            if (_pendingProjectileTimer > 0f)
            {
                _pendingProjectileTimer -= Time.deltaTime;
            }

            // Movements & attacks
            if (VNManager.Instance.IsPlayingStory) // Whatever the situation, we don't do anything the VN phase!
            {
                _rb.velocity = new(0f, _rb.velocity.y, 0f);
            }
            else if (HookTarget != null) // Our hook grabbed a fish
            {
                var dir = (HookTarget.transform.position - transform.position).normalized;
                HookTarget.transform.Translate(dir * _info.HookSpeed);

                if (Vector3.Distance(transform.position, HookTarget.transform.position) < 1f)
                {
                    HookTarget.IsStunned = false;
                    HookTarget = null;
                }
            }
            else if (_target == null || _attackTarget != null) // Do nothing
            {
                _rb.velocity = new(0f, _rb.velocity.y, 0f);
            }
            else
            {
                if (Vector3.Distance(_target.transform.position, transform.position) < _info.Range) // Start attack toward player
                {
                    var dir = (_target.transform.position - transform.position).normalized;
                    var pos = transform.position + (dir * _info.Range) + (dir * _sr.transform.localScale.x * 10f);
                    pos = new(pos.x, 0.19f, pos.z);
                    _sr.flipX = transform.position.x - pos.x > 0f;

                    // Display attack hint
                    _attackTarget = Instantiate(_hintCircle, pos, _hintCircle.transform.rotation);
                    _attackTarget.transform.localScale = new(_info.Range * 2f, _info.Range * 2f, 1f);
                    _rb.velocity = new(0f, _rb.velocity.y, 0f);

                    StartCoroutine(WaitAndAttack());
                }
                else // Move toward player
                {
                    var dir = (_target.transform.position - transform.position).normalized;
                    _rb.velocity = dir * _info.Speed;
                    if (dir.x != 0f)
                    {
                        _sr.flipX = dir.x < 0f;
                    }
                }
            }
        }

        public void OnEnemyUnregistered(EnemyController value)
        {
            // Enemy died, we attempt to remove it from our list in case it's there
            _inRange.Remove(value);
        }

        private IEnumerator WaitAndAttack()
        {
            _anim.SetBool("IsAttacking", true);
            yield return new WaitForSeconds(_info.PreAttackWaitTime);
            _anim.SetBool("IsAttacking", false);

            if (_attackTarget != null) // We didn't got charmed mid attack
            {
                if (!VNManager.Instance.IsPlayingStory) // Only hit if the player didn't load a story meanwhile
                {
                    if (_info.AttackType == AIAttack.Punch)
                    {
                        if (_info.MainAttackVfx)
                        {
                            Destroy(Instantiate(_info.MainAttackVfx, _attackTarget.transform.position, _info.MainAttackVfx.transform.rotation), 1f);
                        }

                        // Attempt to hit player
                        var colliders = Physics.OverlapSphere(_attackTarget.transform.position, _info.Range, _characterMask);
                        foreach (var collider in colliders)
                        {
                            // DEBUG
                            if (collider.gameObject.GetInstanceID() == gameObject.GetInstanceID())
                            {
                                Debug.LogWarning("Enemy is attacking himself!");
                            }
                            var other = collider.GetComponent<ACharacter>();
                            if (other.Team != Team)
                            {
                                other.TakeDamage(_info.AttackForce);
                            }
                        }
                    }
                    else if (_info.AttackType == AIAttack.Hook)
                    {
                        Assert.IsNotNull(_info.MainAttackVfx, "Projectile must be set in Main Attack Vfx");

                        // Spawn and throw projectile
                        var projectile = Instantiate(_info.MainAttackVfx, _attackTarget.transform.position + Vector3.up, _info.MainAttackVfx.transform.rotation);
                        projectile.GetComponent<Rigidbody>().AddForce(transform.forward * _info.ProjectileSpeed);

                        // We wait for the projectile to hit or timeout
                        IsWaitingForProjectile = true;
                        HookTarget = null;
                        _pendingProjectileTimer = _info.ProjectileMaxDistance / _info.ProjectileSpeed;
                        while (IsWaitingForProjectile && _pendingProjectileTimer > 0f)
                        {
                            yield return new WaitForNextFrameUnit();
                        }
                        Destroy(projectile);

                        // Grab target if we hit it
                        if (HookTarget != null)
                        {
                            HookTarget.IsStunned = true;
                        }
                    }
                    else
                    {
                        throw new NotImplementedException($"Unknown attack type {_info.AttackType}");
                    }
                }

                Destroy(_attackTarget);
                _attackTarget = null;
            }
        }

        public override void Die()
        {
            PersistentData.PendingEnergy += Mathf.CeilToInt(1f + 1f * GameManager.Instance.GetStatValue(UpgradableStat.EnergyGained, GameManager.Instance.Info.EnergyCurveGain, GameManager.Instance.Info.MaxEnergyMultiplierGain));
            GameManager.Instance.UpdateUI();
            EnemyManager.Instance.Unregister(this);
        }

        private void OnDestroy()
        {
            if (_attackTarget != null)
            {
                Destroy(_attackTarget);
            }
        }
    }
}
