using Cinemachine;
using LewdieJam.Achievement;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace LewdieJam.Minigame
{
    public class MPlayerController : MonoBehaviour
    {
        private Rigidbody _rb;

        private float _zDest;
        private float _zOffset = 12f;
        private const float _speed = 10f;

        private bool _isDead;

        [SerializeField]
        private CinemachineVirtualCamera _cam;

        [SerializeField]
        private GameObject _accidentVfx;

        [SerializeField]
        private GameObject _victoryPopup;

        [SerializeField]
        private GameObject _exitText;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Car") && !_isDead)
            {
                _exitText.gameObject.SetActive(true);

                _cam.LookAt = null;
                _cam.Follow = null;
                _isDead = true;

                Instantiate(_accidentVfx, collision.contacts[0].point, _accidentVfx.transform.rotation);

                var or = transform.position;
                var car = collision.collider.transform.position;
                _rb.AddForce((new Vector3(or.x - car.x, 0, 0).normalized + Vector3.up) * 50f, ForceMode.Impulse);

                StartCoroutine(WaitAndRetry());
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Victory"))
            {
                _victoryPopup.SetActive(true);
                _rb.velocity = Vector3.zero;
                AchievementManager.Instance.Unlock(AchievementID.Minigame);
            }
            else if (other.CompareTag("Checkpoint"))
            {
                _zOffset += 12f;
            }
        }

        private IEnumerator WaitAndRetry()
        {
            yield return new WaitForSeconds(2f);

            SceneManager.LoadScene("Minigame");
        }

        private void FixedUpdate()
        {
            if (!_isDead && !_victoryPopup.activeInHierarchy)
            {
                if (transform.position.z > _zDest)
                {
                    _rb.velocity = Vector3.zero;
                }
                else
                {
                    _rb.velocity = Vector3.forward * _speed;
                }
            }
        }

        public void OnJump(InputAction.CallbackContext value)
        {
            if (value.performed && !_isDead && !_victoryPopup.activeInHierarchy)
            {
                _zDest += _zOffset;
            }
        }

        public void OnComplete(InputAction.CallbackContext value)
        {
            if (value.performed)
            {
                Complete();
            }
        }

        public void Complete()
        {
            SceneManager.LoadScene("Gallery");
        }
    }
}
