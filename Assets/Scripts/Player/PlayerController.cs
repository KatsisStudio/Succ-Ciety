using LewdieJam.Map;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace LewdieJam.Player
{
    public class PlayerController : ACharacter
    {
        [SerializeField]
        private GameObject _attackVfx;

        public IInteractible CurrentInteraction { set; private get; }

        private Vector2 _mov;

        private void Awake()
        {
            AwakeParent();
        }

        private void FixedUpdate()
        {
            _rb.velocity =
                GameManager.Instance.CanPlay
                ? _info.Speed * Time.fixedDeltaTime * new Vector3(_mov.x, _rb.velocity.y, _mov.y)
                : new(0f, _rb.velocity.y, 0f);
        }

        public override void Die()
        {
            SceneManager.LoadScene("Lobby");
        }

        public void OnMovement(InputAction.CallbackContext value)
        {
            _mov = value.ReadValue<Vector2>();
        }

        public void OnFire(InputAction.CallbackContext value)
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
                        if (collider.CompareTag("Enemy"))
                        {
                            collider.GetComponent<ACharacter>().TakeDamage(1);
                        }
                    }
                }
            }
        }

        public void OnAction(InputAction.CallbackContext value)
        {
            if (value.performed && CurrentInteraction != null && CurrentInteraction.CanInteract(this))
            {
                CurrentInteraction.Interact();
            }
        }
    }
}