using Assets.Scripts.Player;
using LewdieJam.Game;
using LewdieJam.Map;
using LewdieJam.SO;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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

        private bool _isCharmed;
        public bool IsCharmed
        {
            set
            {
                _isCharmed = value;

                EnemyManager.Instance.RefreshAllTargets();

                Instantiate(_charmedPrefab, transform);
            }
            get => _isCharmed;
        }

        public override Team Team => IsCharmed ? Team.Allie : Team.Enemy;

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
                    var pos = transform.position + (_target.transform.position - transform.position).normalized * _info.Range * 1.5f;
                    pos = new(pos.x, 0.01f, pos.z);

                    // Display attack hint
                    _attackTarget = Instantiate(_hintCircle, pos, _hintCircle.transform.rotation);
                    _attackTarget.transform.localScale = new(_info.Range * 2f, _info.Range * 2f, 1f);
                    _rb.velocity = new(0f, _rb.velocity.y, 0f);

                    StartCoroutine(WaitAndAttack());
                }
                else // Move toward player
                {
                    _rb.velocity = (_target.transform.position - transform.position).normalized * _info.Speed * Time.deltaTime;
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
            yield return new WaitForSeconds(1f);

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

        public override void Die()
        {
            PersistentData.PendingEnergy += Mathf.CeilToInt(1f * GameManager.Instance.GetStatValue(UpgradableStat.EnergyGained, GameManager.Instance.Info.EnergyCurveGain, GameManager.Instance.Info.MaxEnergyMultiplierGain));
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
