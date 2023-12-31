﻿using LewdieJam.VN;
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
            _sr.flipX = transform.position.x - pos.x < 0f;

            // Display attack hint
            _attackTarget = Instantiate(_hintCircle, pos, _hintCircle.transform.rotation);
            _attackTarget.transform.localScale = new(_info.Range / 7f, _info.Range / 7f, 1f);
            _attackTarget.GetComponent<SpriteRenderer>().color = _info.IsBoss ? Color.red : new Color(226f / 255f, 0f, 195f / 255f);
            _rb.velocity = new(0f, _rb.velocity.y, 0f);

            StartCoroutine(WaitAndAttack());
        }

        private IEnumerator WaitAndAttack()
        {
            _isAttacking = true;

            if (_info.IsBoss)
            {
                _anim.SetBool("IsWalking", false);
                _anim.SetInteger("AttackState", 1);
            }
            yield return new WaitForSeconds(_info.PreAttackWaitTime - .325f);
            if (_info.SubAttackVfx != null)
            {
                var direction = transform.position - _attackTarget.transform.position;
                direction.y = 0f;

                var dickVfx = Instantiate(_info.SubAttackVfx.gameObject, _attackTarget.transform.position + Vector3.up * 2f, _info.SubAttackVfx.transform.rotation);
                dickVfx.transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                dickVfx.transform.rotation = Quaternion.Euler(dickVfx.transform.rotation.eulerAngles.x, dickVfx.transform.rotation.eulerAngles.y + 180f, dickVfx.transform.rotation.eulerAngles.z);
                Destroy(dickVfx, .325f);
            }
            if (_info.IsBoss)
            {
                _anim.SetInteger("AttackState", 2);
            }
            yield return new WaitForSeconds(.325f);
            if (_info.IsBoss)
            {
                _anim.SetInteger("AttackState", 0);
            }

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
                            other.TakeDamage(this, _info.AttackForce);
                        }
                    }
                }

                Destroy(_attackTarget);
                _attackTarget = null;

                yield return new WaitForSeconds(_info.MainAttackReloadTime);
                _isAttacking = false;
            }
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

        private void OnDestroy()
        {
            if (_attackTarget != null)
            {
                Destroy(_attackTarget);
            }
        }

        private void OnDrawGizmos()
        {
            if (_attackTarget != null)
            {
                Gizmos.color = new Color(1f, 0f, 0f, .2f);
                Gizmos.DrawSphere(_attackTarget.transform.position, _info.Range);
            }
        }
    }
}
