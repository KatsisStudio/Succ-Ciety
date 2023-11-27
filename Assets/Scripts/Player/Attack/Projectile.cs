using LewdieJam.Player.EnemyImpl;
using UnityEngine;

namespace LewdieJam.Player.Attack
{
    public class Projectile : MonoBehaviour
    {
        public EnemyHookController Owner { set; get; }

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent<ACharacter>(out var c) && c.Team != Owner.Team)
            {
                Owner.HookTarget = c;
            }
            Owner.IsWaitingForProjectile = false;
            Destroy(gameObject);
        }
    }
}
