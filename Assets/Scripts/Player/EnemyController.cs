using LewdieJam.Map;
using System.Collections;
using UnityEngine;

namespace LewdieJam.Player
{
    public class EnemyController : ACharacter
    {
        private ACharacter _target;

        [SerializeField]
        private GameObject _hintCircle;

        private GameObject _attackTarget;

        private const float _range = 2f;

        private void Awake()
        {
            AwakeParent();

            var trigger = GetComponentInChildren<TriggerListener>();
            trigger.OnTriggerEnterCallback.AddListener((coll) =>
            {
                if (coll.CompareTag("Player"))
                {
                    _target = coll.GetComponent<ACharacter>();
                }
            });
            trigger.OnTriggerExitCallback.AddListener((coll) =>
            {
                if (coll.CompareTag("Player"))
                {
                    _target = null;
                }
            });
        }

        private void Update()
        {
            if (_target == null || _attackTarget != null)
            {
                _rb.velocity = new(0f, _rb.velocity.y, 0f);
            }
            else
            {
                if (Vector3.Distance(_target.transform.position, transform.position) < _range)
                {
                    var pos = transform.position + (_target.transform.position - transform.position).normalized * _range;
                    pos = new(pos.x, 0.01f, pos.z);
                    _attackTarget = Instantiate(_hintCircle, pos, _hintCircle.transform.rotation);
                    _rb.velocity = new(0f, _rb.velocity.y, 0f);
                    StartCoroutine(WaitAndAttack());
                }
                else
                {
                    _rb.velocity = (_target.transform.position - transform.position).normalized * _info.Speed * Time.deltaTime;
                }
            }
        }

        private IEnumerator WaitAndAttack()
        {
            yield return new WaitForSeconds(1f);
            Destroy(_attackTarget);
            _attackTarget = null;
        }

        public override void Die()
        {
            GameManager.Instance.Energy++;
        }
    }
}
