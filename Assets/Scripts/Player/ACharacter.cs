using UnityEngine;

namespace LewdieJam.Player
{
    public abstract class ACharacter : MonoBehaviour
    {
        [SerializeField]
        protected SO.CharacterInfo _info;

        protected Rigidbody _rb;

        protected int _health;

        protected int _floorMask;
        protected int _playerMask;
        protected int _enemyMask;

        protected virtual int MaxHealth => _info.BaseHealth;

        protected void AwakeParent()
        {
            _floorMask = 1 << LayerMask.NameToLayer("Floor");
            _playerMask = 1 << LayerMask.NameToLayer("Player");
            _enemyMask = 1 << LayerMask.NameToLayer("Enemy");

            _rb = GetComponent<Rigidbody>();
        }

        protected void StartParent()
        {
            _health = MaxHealth;
        }

        protected virtual bool CanTakeDamage => true;

        public virtual void TakeDamage(int damage)
        {
            if (!CanTakeDamage || damage == 0)
            {
                return;
            }

            _health -= damage;
            if (_health <= 0)
            {
                Die();
                Destroy(gameObject);
            }
        }

        public virtual void Die()
        { }
    }
}
