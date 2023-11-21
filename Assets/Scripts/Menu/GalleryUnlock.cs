using LewdieJam.Persistency;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LewdieJam.Menu
{
    public class GalleryUnlock : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField]
        private GameObject _helperText;

        [SerializeField]
        private Button _me;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (!PersistencyManager.Instance.SaveData.DidWinGame)
            {
                _helperText.SetActive(true);
                _me.interactable = false;
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (!PersistencyManager.Instance.SaveData.DidWinGame)
            {
                _helperText.SetActive(false);
                _me.interactable = true;
            }
        }
    }
}
