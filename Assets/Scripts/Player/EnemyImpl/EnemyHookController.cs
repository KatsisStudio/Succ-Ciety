using LewdieJam.Player.Attack;
using LewdieJam.VN;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Assertions;

namespace LewdieJam.Player.EnemyImpl
{
    public class EnemyHookController : AEnemyController
    {
        public bool IsWaitingForProjectile { set; private get; }
        public PlayerController HookTarget { set; private get; }
        private float _pendingProjectileTimer;

        private GameObject _hookProjectile;

        protected override void Attack(Vector3 dir)
        {
            var pos = transform.position + (dir * _sr.transform.localScale.x * 15f);
            _sr.flipX = transform.position.x - pos.x > 0f;

            StartCoroutine(WaitAndAttack(pos));
        }

        protected override bool UpdateSetAction()
        {
            // Update timer
            if (_pendingProjectileTimer > 0f)
            {
                _pendingProjectileTimer -= Time.deltaTime;
            }

            if (HookTarget != null) // Our hook grabbed a fish
            {
                var dir = (HookTarget.transform.position - transform.position).normalized;
                HookTarget.transform.Translate(dir * _info.HookSpeed);

                if (Vector3.Distance(transform.position, HookTarget.transform.position) < 1f)
                {
                    HookTarget.IsStunned = false;
                    HookTarget = null;
                }
                return true;
            }
            return false;
        }

        private IEnumerator WaitAndAttack(Vector3 atkPos)
        {
            _isAttacking = true;
            _anim.SetBool("IsAttacking", true);
            yield return new WaitForSeconds(_info.PreAttackWaitTime);
            _anim.SetBool("IsAttacking", false);

            if (!VNManager.Instance.IsPlayingStory) // Only hit if the player didn't load a story meanwhile
            {
                Assert.IsNotNull(_info.MainAttackVfx, "Projectile must be set in Main Attack Vfx");

                // Spawn and throw projectile
                _hookProjectile = Instantiate(_info.MainAttackVfx, atkPos, _info.MainAttackVfx.transform.rotation);
                _hookProjectile.GetComponent<Rigidbody>().AddForce(transform.forward * _info.ProjectileSpeed);
                _hookProjectile.GetComponent<Projectile>().Owner = this;

                // We wait for the projectile to hit or timeout
                IsWaitingForProjectile = true;
                HookTarget = null;
                _pendingProjectileTimer = _info.ProjectileMaxDistance / _info.ProjectileSpeed;
                while (IsWaitingForProjectile && _pendingProjectileTimer > 0f)
                {
                    yield return new WaitForNextFrameUnit();
                }
                Destroy(_hookProjectile);

                // Grab target if we hit it
                if (HookTarget != null)
                {
                    HookTarget.IsStunned = true;
                }
            }

            yield return new WaitForSeconds(_info.MainAttackReloadTime);
            _isAttacking = false;
        }

        private void OnDestroy()
        {
            if (_hookProjectile != null)
            {
                Destroy(_hookProjectile);
            }
        }
    }
}
