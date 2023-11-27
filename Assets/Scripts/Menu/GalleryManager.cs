using LewdieJam.Game;
using LewdieJam.Lobby;
using LewdieJam.SO;
using LewdieJam.VN;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

namespace LewdieJam.Menu
{
    public class GalleryManager : MonoBehaviour
    {
        [SerializeField]
        private Live2DManager _live2d;

        [SerializeField]
        private AudioSource _bgm;

        [SerializeField]
        private GameObject _oldLobby;

        private int _hornLevel;

        private void Awake()
        {
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
        }

        private void Start()
        {
            _oldLobby.SetActive(false);
        }

        public void PlayBGM(AudioClip clip)
        {
            if (_bgm.clip == clip)
            {
                _bgm.Stop();
            }
            else
            {
                _bgm.clip = clip;
                _bgm.Play();
            }
        }

        public void BackToMenu()
        {
            SceneManager.LoadScene("Menu");
        }

        public void LoadSecret()
        {
            SceneManager.LoadScene("Minigame");
        }

        public void LoadHScene(HSceneInfo scene)
        {
            _bgm.Stop();
            VNManager.Instance.ShowHSceneStory(scene);
        }

        public void OnClick(InputAction.CallbackContext value)
        {
            if (value.performed && VNManager.Instance.IsPlayingStory)
            {
                VNManager.Instance.DisplayNextDialogue();
            }
        }
        public void ToggleLargeBreastsAttachment() => _live2d.ToggleAttachment(Attachment.LargeBreasts);
        public void ToggleFutanariAttachment() => _live2d.ToggleAttachment(Attachment.Futanari);
        public void TogglePregnantAttachment() => _live2d.ToggleAttachment(Attachment.Pregnant);

        public void HornLevelUp()
        {
            if (_hornLevel < 5)
            {
                _hornLevel++;
                _live2d.SetHornLevel(_hornLevel);
            }
        }
        public void HornLevelDown()
        {
            if (_hornLevel > 0)
            {
                _hornLevel--;
                _live2d.SetHornLevel(_hornLevel);
            }
        }
    }
}
