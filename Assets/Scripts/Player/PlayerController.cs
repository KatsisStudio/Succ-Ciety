using UnityEngine;
using UnityEngine.InputSystem;
using YuriGameJam2023.SO;

namespace LewdieJam
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInfo _info;

        private Vector2 _mov;

        private Rigidbody _rb;

        private void Awake()
        {
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
                // TODO
            }
        }
    }
}