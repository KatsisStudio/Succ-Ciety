using UnityEngine;

namespace LewdieJam.Player
{
    public abstract class ACharacter : MonoBehaviour
    {
        [SerializeField]
        protected SO.CharacterInfo _info;

        protected Rigidbody _rb;

        private int _health;

        protected void AwakeParent()
        {
            _rb = GetComponent<Rigidbody>();
            _health = _info.BaseHealth;
        }

        public void TakeDamage(int damage)
        {
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
