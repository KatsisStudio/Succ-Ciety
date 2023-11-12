using Assets.Scripts.Player;
using LewdieJam.Game;
using LewdieJam.Map;
using LewdieJam.SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.PlayerSettings;

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
        private SpriteRenderer _sr;

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
            _sr = GetComponentInChildren<SpriteRenderer>();

            // Ignore player if out of range
            var trigger = GetComponentInChildren<TriggerListener>();
            trigger.OnTriggerEnterCallback.AddListener((coll) =>
            {
                if (coll.CompareTag("Player") || coll.CompareTag("Enemy"))
                {
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
            if (_target == null || _attackTarget != null) // Do nothing
            {
                _rb.velocity = new(0f, _rb.velocity.y, 0f);
            }
            else
            {
                if (Vector3.Distance(_target.transform.position, transform.position) < _info.Range) // Start attack toward player
                {
                    var dir = (_target.transform.position - transform.position).normalized;
                    var pos = transform.position + (dir * _info.Range) + dir;
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
