﻿using Cinemachine;
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
        private const float _zOffset = 12f;
        private const float _speed = 10f;

        private bool _isDead;

        [SerializeField]
        private CinemachineVirtualCamera _cam;

        [SerializeField]
        private GameObject _accidentVfx;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (collision.collider.CompareTag("Car") && !_isDead)
            {
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

        private IEnumerator WaitAndRetry()
        {
            yield return new WaitForSeconds(3f);

            SceneManager.LoadScene("Minigame");
        }

        private void FixedUpdate()
        {
            if (!_isDead)
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
            if (value.performed && !_isDead)
            {
                _zDest += _zOffset;
            }
        }
    }
}
