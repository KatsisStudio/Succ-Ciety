using LewdieJam.Player;
using System.Runtime.CompilerServices;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LewdieJam.Map
{
    public class GoHomeZone : MonoBehaviour, IInteractible
    {
        [SerializeField]
        private GameObject _actionText;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _actionText.SetActive(true);
                other.GetComponent<PlayerController>().CurrentInteraction = this;
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                _actionText.SetActive(false);
                other.GetComponent<PlayerController>().CurrentInteraction = null;
            }
        }

        public bool CanInteract(PlayerController pc)
        {
            return true;
        }

        public void Interact()
        {
            SceneManager.LoadScene("Lobby");
        }
    }
}
