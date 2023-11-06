using LewdieJam.Game;
using LewdieJam.Map;
using LewdieJam.SO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LewdieJam.Player
{
    public class PlayerController : ACharacter
    {
        [SerializeField]
        private GameObject _attackVfx;

        [SerializeField]
        private Image _healthBar;

        public IInteractible CurrentInteraction { set; private get; }

        private Vector2 _mov;

        /// <summary>
        /// When taking a hit, need to wait a bit before being able to take a new one
        /// </summary>
        private bool _isInvulnerabilityFrame;

        private float _invultDuration = 2f;
        private float _invultFrameIntensity = .5f;

        private SpriteRenderer _sr;

        protected override int MaxHealth => _info.BaseHealth * (int)GameManager.Instance.GetStatValue(UpgradableStat.BaseHealth, GameManager.Instance.Info.MaxHealthCurveGain, GameManager.Instance.Info.MaxHealthMultiplerGain);

        private bool _canUseUltimate = true;

        private void Awake()
        {
            AwakeParent();
            _sr = GetComponentInChildren<SpriteRenderer>();
        }

        private void Start()
        {
            StartParent();
        }

        private void FixedUpdate()
        {
            _rb.velocity =
                GameManager.Instance.CanPlay
                ? _info.Speed * Time.fixedDeltaTime * new Vector3(_mov.x, _rb.velocity.y, _mov.y)
                : new(0f, _rb.velocity.y, 0f);
        }

        protected override bool CanTakeDamage => !_isInvulnerabilityFrame;

        public override void TakeDamage(int damage)
        {
            base.TakeDamage(damage);

            if (damage > 0)
            {
                StartCoroutine(DisplayInvulnerability());
                _healthBar.transform.localScale = new Vector3(_health / (float)MaxHealth, 1f, 1f);
            }
        }

        private IEnumerator DisplayInvulnerability()
        {
            _isInvulnerabilityFrame = true;
            bool state = true;
            for (float i = 0; i < _invultDuration; i += _invultFrameIntensity)
            {
                yield return new WaitForSeconds(_invultFrameIntensity);
                state = !state;
                _sr.color = state ? Color.white : new Color(1f, 1f, 1f, 0f);
            }

            _sr.color = Color.white;
            _isInvulnerabilityFrame = false;
        }

        public override void Die()
        {
            PersistentData.Energy += PersistentData.PendingEnergy / 2;
            PersistentData.PendingEnergy = 0;
            SceneManager.LoadScene("Lobby");
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            _mov = value.ReadValue<Vector2>();
        }

        private IEnumerable<ACharacter> FireOnTarget()
        {
            if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, _floorMask))
            {
                Debug.Log("fire in the hole");
                // Calculate mouse forward vector
                var direction = hit.point - transform.position;
                direction.y = 0f;
                var dirNorm = direction.normalized;
                var targetPos = transform.position + dirNorm * _info.Range;

                // Spawn VFX
                Destroy(Instantiate(_attackVfx, targetPos, _attackVfx.transform.rotation), 1f);

                // Damage all enemies in range
                var colliders = Physics.OverlapSphere(targetPos, _info.Range, _enemyMask);
                foreach (var collider in colliders)
                {
                    if (collider.CompareTag("Enemy"))
                    {
                        yield return collider.GetComponent<ACharacter>();
                    }
                }
            }
        }

        public void OnFire(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                var damage = 1 + Mathf.CeilToInt(GameManager.Instance.GetStatValue(UpgradableStat.AtkPower, GameManager.Instance.Info.AtkCurveGain, GameManager.Instance.Info.MaxAtkMultiplerGain));
                foreach (var coll in FireOnTarget())
                {
                    coll.TakeDamage(damage);
                }
            }
        }

        public void OnUltimate(InputAction.CallbackContext value)
        {
            if (value.performed && _canUseUltimate)
            {
                foreach (var coll in FireOnTarget())
                {
                    var ec = (EnemyController)coll;
                    if (!ec.IsCharmed)
                    {
                        ec.IsCharmed = true;
                    }
                }
            }
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (value.performed && CurrentInteraction != null && CurrentInteraction.CanInteract(this))
            {
                CurrentInteraction.Interact();
            }
        }
    }
}