using UnityEngine;

namespace LewdieJam.Player
{
    public abstract class ACharacter : MonoBehaviour
    {
        [SerializeField]
        protected SO.CharacterInfo _info;

        protected Rigidbody _rb;

        protected int _health;

        protected virtual int MaxHealth => _info.BaseHealth;

        protected void AwakeParent()
        {
            _rb = GetComponent<Rigidbody>();
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
