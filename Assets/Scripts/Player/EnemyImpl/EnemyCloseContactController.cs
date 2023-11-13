using LewdieJam.VN;
using System.Collections;
using UnityEngine;

namespace LewdieJam.Player.EnemyImpl
{
    public class EnemyCloseContactController : AEnemyController
    {
        [SerializeField]
        private GameObject _hintCircle;

        private GameObject _attackTarget;

        protected override void Attack(Vector3 dir)
        {
            var pos = transform.position + (dir * _info.Range) + (dir * _sr.transform.localScale.x * 10f);
            pos = new(pos.x, 0.19f, pos.z);
            _sr.flipX = transform.position.x - pos.x > 0f;

            // Display attack hint
            _attackTarget = Instantiate(_hintCircle, pos, _hintCircle.transform.rotation);
            _attackTarget.transform.localScale = new(_info.Range * 2f, _info.Range * 2f, 1f);
            _rb.velocity = new(0f, _rb.velocity.y, 0f);

            StartCoroutine(WaitAndAttack());
        }

        protected override void OnCharmed()
        {
            base.OnCharmed();

            if (_attackTarget != null)
            {
                Destroy(_attackTarget);
                _attackTarget = null;
            }
        }

        private IEnumerator WaitAndAttack()
        {
            _isAttacking = true;

            _anim.SetBool("IsAttacking", true);
            yield return new WaitForSeconds(_info.PreAttackWaitTime);
            _anim.SetBool("IsAttacking", false);

            if (_attackTarget != null) // We didn't got charmed mid attack
            {
                if (!VNManager.Instance.IsPlayingStory) // Only hit if the player didn't load a story meanwhile
                {
                    if (_info.MainAttackVfx)
                    {
                        Destroy(Instantiate(_info.MainAttackVfx, _attackTarget.transform.position, _info.MainAttackVfx.transform.rotation), 1f);
                    }

                    // Attempt to hit player
                    var colliders = Physics.OverlapSphere(_attackTarget.transform.position, _info.Range, _characterMask);
                    foreach (var collider in colliders)
                    {
                        // DEBUG
                        if (collider.gameObject.GetInstanceID() == gameObject.GetInstanceID())
                        {
                            Debug.LogWarning("Enemy is attacking himself!");
                        }
                        var other = collider.GetComponent<ACharacter>();
                        if (other.Team != Team)
                        {
                            other.TakeDamage(_info.AttackForce);
                        }
                    }
                }

                Destroy(_attackTarget);
                _attackTarget = null;

                yield return new WaitForSeconds(_info.MainAttackReloadTime);
                _isAttacking = false;
            }
        }

        private void OnDestroy()
        {
            if (_attackTarget != null)
            {
                Destroy(_attackTarget);
            }
        }
    }
}
