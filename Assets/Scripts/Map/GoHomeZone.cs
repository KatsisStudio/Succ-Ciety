using LewdieJam.Game;
using LewdieJam.Persistency;
using LewdieJam.Player;
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
            PersistencyManager.Instance.SaveData.Energy += PersistencyManager.Instance.SaveData.PendingEnergy;
            PersistencyManager.Instance.SaveData.PendingEnergy = 0;
            PersistencyManager.Instance.Save();
            SceneManager.LoadScene("Lobby");
        }
    }
}
