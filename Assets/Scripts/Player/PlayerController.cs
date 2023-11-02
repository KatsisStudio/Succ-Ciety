using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.TextCore.Text;
using YuriGameJam2023.SO;

namespace LewdieJam
{
    public class PlayerController : MonoBehaviour
    {
        [SerializeField]
        private PlayerInfo _info;

        [SerializeField]
        private GameObject _attackVfx;

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

                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out var hit, Mathf.Infinity, 1 << 6))
                {
                    var direction = hit.point - transform.position;
                    direction.y = 0f;

                    var dirNorm = direction.normalized;

                    Destroy(Instantiate(_attackVfx, transform.position + dirNorm * 2f, _attackVfx.transform.rotation), 1f);
                }

                /*var colliders = Physics.OverlapSphere(transform.position, 1f);
                foreach (var collider in colliders)
                {
                }*/
            }
        }
    }
}