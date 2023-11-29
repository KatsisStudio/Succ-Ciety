using Assets.Scripts.Player;
using LewdieJam.Player.EnemyImpl;
using UnityEngine;
using UnityEngine.UI;

namespace LewdieJam.Player
{
    public abstract class ACharacter : MonoBehaviour
    {
        [SerializeField]
        protected SO.CharacterInfo _info;

        [SerializeField]
        protected Image _healthBar;

        protected Rigidbody _rb;
        protected SpriteRenderer _sr;

        protected int _health;

        protected int _floorMask;
        protected int _characterMask;

        public EnemyHookController Hooker { set; get; }

        protected virtual int MaxHealth => _info.BaseHealth;

        public abstract Team Team { get; }

        protected bool _isAttacking;

        public bool IsAlive => _health > 0; // No use targetting player after her death

        protected void AwakeParent()
        {
            _floorMask = 1 << LayerMask.NameToLayer("Floor");
            _characterMask = 1 << LayerMask.NameToLayer("Character");

            _rb = GetComponent<Rigidbody>();
            _sr = GetComponentInChildren<SpriteRenderer>();
        }

        protected void StartParent()
        {
            _health = MaxHealth;
        }

        public static void UpdateSrSortingOrder(Transform t, SpriteRenderer sr)
        {
            var targetPos = (t.rotation.eulerAngles.y > 45 && t.rotation.eulerAngles.y < 135f)
                || (t.rotation.eulerAngles.y > 225 && t.rotation.eulerAngles.y < 315)
                ? t.position.x : t.position.z;
            sr.sortingOrder = (int)(-targetPos * 100f + 10_000_000f);
        }

        protected void UpdateParent()
        {
            UpdateSrSortingOrder(transform, _sr);
        }

        protected virtual bool CanTakeDamage => true;

        public virtual void TakeDamage(ACharacter source, int damage)
        {
            if (!CanTakeDamage || damage == 0)
            {
                return;
            }

            _health -= damage;
            if (_health < 0) _health = 0;
            if (_health == 0)
            {
                Die();
                EnemyManager.Instance.RefreshAllTargets();
            }

            if (_healthBar != null)
            {
                _healthBar.transform.localScale = new Vector3(_health / (float)MaxHealth, 1f, 1f);
                _healthBar.gameObject.SetActive(true);
            }
        }

        public virtual void Die()
        { }
    }
}
