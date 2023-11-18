using LewdieJam.Map;
using LewdieJam.Persistency;
using LewdieJam.Player;
using LewdieJam.SO;
using LewdieJam.VN;
using UnityEngine;

namespace LewdieJam.Achievement
{
    public class ManholeCover : MonoBehaviour, IInteractible
    {
        [SerializeField]
        private GameObject _actionText;

        [SerializeField]
        private HSceneInfo _hScene;

        public bool CanInteract(PlayerController pc)
        {
            return PersistencyManager.Instance.SaveData.TokenFoundCount == AchievementManager.Instance.CurrentTokenCount;
        }

        public void Interact()
        {
            VNManager.Instance.InitHScene(_hScene);
            GameManager.Instance.PlayHScene();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player") && PersistencyManager.Instance.SaveData.TokenFoundCount == AchievementManager.Instance.CurrentTokenCount)
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
    }
}
