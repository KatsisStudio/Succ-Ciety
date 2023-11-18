using Assets.Scripts.Player;
using UnityEngine;

namespace LewdieJam.Player
{
    public abstract class ACharacter : MonoBehaviour
    {
        [SerializeField]
        protected SO.CharacterInfo _info;

        protected Rigidbody _rb;
        protected SpriteRenderer _sr;

        protected int _health;

        protected int _floorMask;
        protected int _characterMask;

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

        protected void UpdateParent()
        {
            var targetPos = (transform.rotation.eulerAngles.y > 45 && transform.rotation.eulerAngles.y < 135f)
                || (transform.rotation.eulerAngles.y > 225 && transform.rotation.eulerAngles.y < 315)
                ? transform.position.x : transform.position.z;
            _sr.sortingOrder = (int)(-targetPos * 100f);
        }

        protected virtual bool CanTakeDamage => true;

        public virtual void TakeDamage(ACharacter source, int damage)
        {
            if (!CanTakeDamage || damage == 0)
            {
                return;
            }

            _health -= damage;
            if (_health <= 0)
            {
                Die();
                EnemyManager.Instance.RefreshAllTargets();
            }
        }

        public virtual void Die()
        { }
    }
}
