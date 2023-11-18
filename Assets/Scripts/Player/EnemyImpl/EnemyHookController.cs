using LewdieJam.Achievement;
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
        public ACharacter HookTarget { set; get; }
        private float _pendingProjectileTimer;

        private GameObject _hookProjectile;

        protected override void Attack(Vector3 dir)
        {
            var pos = transform.position + (dir * _sr.transform.localScale.x * 15f);
            _sr.flipX = transform.position.x - pos.x > 0f;

            StartCoroutine(WaitAndAttack(dir, pos));
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
                var dir = (transform.position - HookTarget.transform.position).normalized;
                HookTarget.transform.position = HookTarget.transform.position + dir * _info.HookSpeed * Time.deltaTime;

                if (Vector3.Distance(transform.position, HookTarget.transform.position) < 1f)
                {
                    HookTarget.Hooker = null;
                    HookTarget = null;
                }
                return true;
            }
            return false;
        }

        private IEnumerator WaitAndAttack(Vector3 dir, Vector3 atkPos)
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
                _hookProjectile.GetComponent<Rigidbody>().AddForce(dir * _info.ProjectileSpeed, ForceMode.Impulse);
                _hookProjectile.GetComponent<Projectile>().Owner = this;

                // We wait for the projectile to hit or timeout
                IsWaitingForProjectile = true;
                HookTarget = null;
                _pendingProjectileTimer = _info.ProjectileSpeed / _info.ProjectileMaxDistance;
                while (IsWaitingForProjectile && _pendingProjectileTimer > 0f) // We wait for the ball to touch something or timeout
                {
                    yield return new WaitForNextFrameUnit();
                }
                Destroy(_hookProjectile);

                // Grab target if we hit it
                if (HookTarget != null)
                {
                    if (HookTarget.Hooker != null && HookTarget.Hooker && HookTarget.Hooker.HookTarget is PlayerController)
                    {
                        HookTarget.Hooker.HookTarget = null;
                        AchievementManager.Instance.Unlock(AchievementID.DoubleHook);
                    }
                    HookTarget.Hooker = this;
                }
            }

            yield return new WaitForSeconds(_info.MainAttackReloadTime);
            _isAttacking = false;
        }

        protected override void OnCharmed()
        {
            base.OnCharmed();
            
            if (HookTarget != null)
            {
                HookTarget.Hooker = null;
            }
            HookTarget = null;
            if (_hookProjectile != null)
            {
                Destroy(_hookProjectile);
            }
        }

        private void OnDestroy()
        {
            if (HookTarget != null)
            {
                HookTarget.Hooker = null;
            }
            if (_hookProjectile != null)
            {
                Destroy(_hookProjectile);
            }
        }
    }
}
