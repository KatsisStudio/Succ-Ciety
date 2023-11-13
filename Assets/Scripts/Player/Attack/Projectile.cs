using UnityEngine;

namespace LewdieJam.Player.Attack
{
    public class Projectile : MonoBehaviour
    {
        public EnemyController Owner { set; get; }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Player"))
            {
                Owner.HookTarget = collision.collider.GetComponent<PlayerController>();
            }
            Owner.IsWaitingForProjectile = false;
            Destroy(gameObject);
        }
    }
}
