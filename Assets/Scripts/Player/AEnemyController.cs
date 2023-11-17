using Assets.Scripts.Player;
using LewdieJam.Achievement;
using LewdieJam.Game;
using LewdieJam.Map;
using LewdieJam.SO;
using LewdieJam.VN;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LewdieJam.Player
{
    public abstract class AEnemyController : ACharacter
    {
        private ACharacter _target;

        [SerializeField]
        private GameObject _charmedPrefab;

        [SerializeField]
        private SphereCollider _detector;

        private readonly List<ACharacter> _inRange = new();

        private GameObject _charmedEffect;

        protected Animator _anim;

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

                OnCharmed();
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

        /// <summary>
        /// Recalculate which target we are aiming at
        /// </summary>
        public void RefreshTarget()
        {
            _target = _inRange
                .Where(x => x.Team != Team && x.IsAlive)
                .OrderBy(x => Vector3.Distance(transform.position, x.transform.position))
                .FirstOrDefault();
        }

        private void Awake()
        {
            AwakeParent();

            _anim = GetComponentInChildren<Animator>();
            _detector.radius = _info.DetectionRange;

            // Store all potential targets that are in range
            var trigger = GetComponentInChildren<TriggerListener>();
            trigger.OnTriggerEnterCallback.AddListener((coll) =>
            {
                if (coll.CompareTag("Player") || coll.CompareTag("Enemy"))
                {
                    if (coll.CompareTag("Player") && _info.IsBoss)
                    {
                        GameManager.Instance.EnableBossHealthBar(_info.Name);
                        GameManager.Instance.UpdateHealthBar(_health / (float)_info.BaseHealth);
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

            // Movements & attacks
            if (VNManager.Instance.IsPlayingStory) // Whatever the situation, we don't do anything the VN phase!
            {
                _rb.velocity = new(0f, _rb.velocity.y, 0f);
            }
            else if (UpdateSetAction())
            { }
            else if (_target == null || _isAttacking) // Do nothing
            {
                _rb.velocity = new(0f, _rb.velocity.y, 0f);
            }
            else
            {
                if (Vector3.Distance(_target.transform.position, transform.position) < _info.Range) // Start attack toward player
                {
                    var dir = (_target.transform.position - transform.position).normalized;

                    Attack(dir);
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

        protected abstract void Attack(Vector3 dir);
        protected virtual bool UpdateSetAction()
            => false;

        protected virtual void OnCharmed()
        { }

        public void OnEnemyUnregistered(AEnemyController value)
        {
            // Enemy died, we attempt to remove it from our list in case it's there
            _inRange.Remove(value);
        }

        public override void Die()
        {
            PersistentData.PendingEnergy += Mathf.CeilToInt(_info.EnergyDropped + _info.EnergyDropped * GameManager.Instance.GetStatValue(UpgradableStat.EnergyGained, GameManager.Instance.Info.EnergyCurveGain, GameManager.Instance.Info.MaxEnergyMultiplierGain));
            GameManager.Instance.UpdateUI();
            if (_info.IsBoss)
            {
                GameManager.Instance.DisableBossHealthBar();
                AchievementManager.Instance.Unlock(AchievementID.Dickus);
            }
            EnemyManager.Instance.Unregister(this);
            Destroy(gameObject);
        }
    }
}
