using LewdieJam.Achievement;
using LewdieJam.Game;
using LewdieJam.Lobby;
using LewdieJam.Persistency;
using LewdieJam.SO;
using LewdieJam.VN;
using TMPro;
using Unity.VisualScripting;
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
        private LobbyArtManager _artManager;

        [SerializeField]
        private AudioSource _bgm;

        [SerializeField]
        private GameObject _oldLobby;

        [SerializeField]
        private Transform _achievementContainer;

        [SerializeField]
        private GameObject _achievementPrefab;

        private int _hornLevel;

        private void Awake()
        {
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
        }

        private void Start()
        {
            _oldLobby.SetActive(false);
            _artManager.SetHornLevel(0);
            _artManager.ToggleAttachment(Attachment.None);

            foreach (var achievement in AchievementManager.Instance.Achievements)
            {
                var a = Instantiate(_achievementPrefab, _achievementContainer);
                var txts = a.GetComponentsInChildren<TMP_Text>();
                txts[0].text = PersistencyManager.Instance.SaveData.IsUnlocked(achievement.Key) ? achievement.Value.Name : "???";
                txts[1].text = achievement.Value.Description;
            }
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
        public void ToggleNone() => _artManager.ToggleAttachment(Attachment.None);
        public void ToggleLargeBreastsAttachment() => _artManager.ToggleAttachment(Attachment.LargeBreasts);
        public void ToggleFutanariAttachment() => _artManager.ToggleAttachment(Attachment.Futanari);
        public void TogglePregnantAttachment() => _artManager.ToggleAttachment(Attachment.Pregnant);

        public void HornLevelUp(bool useNewLobby)
        {
            if (_hornLevel < 5)
            {
                _hornLevel++;
                if (useNewLobby) _artManager.SetHornLevel(_hornLevel);
                else _live2d.SetHornLevel(_hornLevel);
            }
        }
        public void HornLevelDown(bool useNewLobby)
        {
            if (_hornLevel > 0)
            {
                _hornLevel--;
                if (useNewLobby) _artManager.SetHornLevel(_hornLevel);
                else _live2d.SetHornLevel(_hornLevel);
            }
        }
    }
}
