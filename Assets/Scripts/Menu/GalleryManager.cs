using LewdieJam.SO;
using LewdieJam.VN;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace LewdieJam.Menu
{
    public class GalleryManager : MonoBehaviour
    {
        private void Awake()
        {
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        public void LoadHScene(HSceneInfo scene)
        {
            VNManager.Instance.ShowHSceneStory(scene);
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed && VNManager.Instance.IsPlayingStory)
            {
                VNManager.Instance.DisplayNextDialogue();
            }
        }
    }
}
