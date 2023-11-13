using LewdieJam;
using UnityEngine;

namespace Assets.Scripts.Game
{
    public class CameraZone : MonoBehaviour
    {
        [SerializeField]
        private float _targetAngle;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.SetRotationAngle(_targetAngle);
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                GameManager.Instance.SetRotationAngle(0f);
            }
        }
    }
}
