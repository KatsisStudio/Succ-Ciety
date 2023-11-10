using Assets.Scripts.Player;
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
        protected int _characterMask;

        protected virtual int MaxHealth => _info.BaseHealth;

        public abstract Team Team { get; }

        protected void AwakeParent()
        {
            _floorMask = 1 << LayerMask.NameToLayer("Floor");
            _characterMask = 1 << LayerMask.NameToLayer("Character");

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
                EnemyManager.Instance.RefreshAllTargets();
            }
        }

        public virtual void Die()
        { }
    }
}
