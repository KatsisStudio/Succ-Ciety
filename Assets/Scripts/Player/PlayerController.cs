using UnityEngine;
using UnityEngine.InputSystem;

namespace LewdieJam.Player
{
    public class PlayerController : ACharacter
    {
        [SerializeField]
        private GameObject _attackVfx;

        private Vector2 _mov;

        private Rigidbody _rb;

        private void Awake()
        {
            AwakeParent();
            _rb = GetComponent<Rigidbody>();
        }

        private void FixedUpdate()
        {
            _rb.velocity = _info.Speed * Time.fixedDeltaTime * new Vector3(_mov.x, _rb.velocity.y, _mov.y);
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            _mov = value.ReadValue<Vector2>();
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, 1 << 6))
                {
                    // Calculate mouse forward vector
                    var direction = hit.point - transform.position;
                    direction.y = 0f;
                    var dirNorm = direction.normalized;
                    var targetPos = transform.position + dirNorm * 2f;

                    // Spawn VFX
                    Destroy(Instantiate(_attackVfx, targetPos, _attackVfx.transform.rotation), 1f);

                    // Damage all enemies in range
                    var colliders = Physics.OverlapSphere(targetPos, 2f, 1 << 8);
                    foreach (var collider in colliders)
                    {
                        collider.GetComponent<ACharacter>().TakeDamage(1);
                    }
                }
            }
        }
    }
}