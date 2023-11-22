using Assets.Scripts.Player;
using LewdieJam.Achievement;
using LewdieJam.Persistency;
using LewdieJam.Player;
using LewdieJam.SO;
using LewdieJam.VN;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace LewdieJam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        [SerializeField]
        private GameInfo _info;

        [SerializeField]
        private GameObject _healthBarContainer;

        [SerializeField]
        private TMP_Text _bossNameText;

        [SerializeField]
        private RectTransform _healthBar;

        [SerializeField]
        private PlayerController _player;

        [SerializeField]
        private AudioSource _bgmGame;

        [SerializeField]
        private TMP_Text _tokenFindInfo;

        [SerializeField]
        private TextAsset _story;

        [SerializeField]
        private AudioClip _gameOverClip;

        public GameInfo Info => _info;

        [SerializeField]
        private TMP_Text _energyDisplay;

        private float _currentAngle;
        private bool _goUp;

        private void Awake()
        {
            Instance = this;
            SceneManager.LoadScene("Map", LoadSceneMode.Additive);
            SceneManager.LoadScene("VN", LoadSceneMode.Additive);
            UpdateUI();
        }

        private void Start()
        {
            if (!PersistencyManager.Instance.SaveData.SawIntro)
            {
                VNManager.Instance.ShowStory(_story, () =>
                {
                    PersistencyManager.Instance.SaveData.SawIntro = true;
                    PersistencyManager.Instance.Save();
                });
            }
        }

        private void Update()
        {
            if (_player.transform.rotation.eulerAngles.y != _currentAngle)
            {
                float a;
                if (_goUp)
                {
                    a = _player.transform.rotation.eulerAngles.y + Time.deltaTime * 100f;
                    if (a > _currentAngle)
                    {
                        a = _currentAngle;
                    }
                }
                else
                {
                    a = _player.transform.rotation.eulerAngles.y - Time.deltaTime * 100f;
                    if (a < _currentAngle)
                    {
                        a = _currentAngle;
                    }
                }
                _player.transform.rotation = Quaternion.Euler(_player.transform.rotation.x, a, _player.transform.rotation.z);
                foreach (var e in EnemyManager.Instance.ToEnumerable())
                {
                    e.transform.rotation = Quaternion.Euler(_player.transform.rotation.x, a, _player.transform.rotation.z);
                }
            }
        }

        public IEnumerator UpdateTokenDisplay()
        {
            _tokenFindInfo.gameObject.SetActive(true);

            if (PersistencyManager.Instance.SaveData.TokenFoundCount == AchievementManager.Instance.CurrentTokenCount)
            {
                _tokenFindInfo.text = "Find the sewer entrance for a <i>special</i> reward";
            }
            else
            {
                var left = AchievementManager.Instance.CurrentTokenCount - PersistencyManager.Instance.SaveData.TokenFoundCount;
                _tokenFindInfo.text = $"{left} token{(left > 1 ? "s" : string.Empty)} left!";
            }

            yield return new WaitForSeconds(5f);

            _tokenFindInfo.gameObject.SetActive(false);
        }

        public void PlayHScene()
        {
            _bgmGame.Pause();
            VNManager.Instance.ShowHSceneStoryWithCallback(() =>
            {
                _bgmGame.UnPause();
                PlayerController.Instance.UpdateTarget();
            }, true);
        }

        public void PlayGameOverBgm()
        {
            _bgmGame.clip = _gameOverClip;
            _bgmGame.Play();
        }

        public void SetRotationAngle(float angle)
        {
            _goUp = angle > _currentAngle;
            _currentAngle = angle;
        }

        public void EnableBossHealthBar(string enemyName)
        {
            _healthBarContainer.SetActive(true);
            _bossNameText.text = enemyName;
            _healthBar.localScale = Vector3.one;
        }

        public void UpdateHealthBar(float value)
        {
            _healthBar.localScale = new(value, 1f, 1f);
        }

        public void DisableBossHealthBar()
        {
            _healthBarContainer.SetActive(false);
        }

        public float GetStatValue(UpgradableStat stat, AnimationCurve curve, float maxVal)
        {
            return curve.Evaluate(PersistencyManager.Instance.SaveData.GetStatValue(stat) / (float)_info.MaxLevel) * maxVal;
        }

        public bool CanPlay => !VNManager.Instance.IsPlayingStory;

        public void UpdateUI()
        {
            _energyDisplay.text = $"Energy: {PersistencyManager.Instance.SaveData.Energy} ({PersistencyManager.Instance.SaveData.PendingEnergy})";
        }
    }
}
