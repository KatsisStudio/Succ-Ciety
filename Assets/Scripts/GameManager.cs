using Assets.Scripts.Player;
using LewdieJam.Game;
using LewdieJam.Player;
using LewdieJam.SO;
using LewdieJam.VN;
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
        private AudioSource _bgmGame, _bgmHScene;

        private float _bgmTimer;
        private float _bgmTimerTarget;

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

        private void Update()
        {
            if (_bgmTimer != _bgmTimerTarget)
            {
                if (_bgmTimer < _bgmTimerTarget)
                {
                    _bgmTimer += Time.deltaTime * .1f;
                    if (_bgmTimer > _bgmTimerTarget)
                    {
                        _bgmTimer = _bgmTimerTarget;
                    }
                }
                else
                {
                    _bgmTimer -= Time.deltaTime * .1f;
                    if (_bgmTimer < _bgmTimerTarget)
                    {
                        _bgmTimer = _bgmTimerTarget;
                    }
                }

                _bgmGame.volume = .4f - _bgmTimer;
                _bgmHScene.volume = _bgmTimer;

                if (_bgmGame.volume == 0f) _bgmGame.Pause();
                if (_bgmHScene.volume == 0f) _bgmHScene.Stop();
            }

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

        public void EnableGameBgm()
        {
            _bgmTimerTarget = 0f;
            _bgmGame.UnPause();
        }

        public void EnableHSceneBgm()
        {
            _bgmTimerTarget = .4f;
            _bgmHScene.Play();
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
            return curve.Evaluate(PersistentData.GetStatValue(stat) / (float)_info.MaxLevel) * maxVal;
        }

        public bool CanPlay => !VNManager.Instance.IsPlayingStory;

        public void UpdateUI()
        {
            _energyDisplay.text = $"Energy: {PersistentData.Energy} ({PersistentData.PendingEnergy})";
        }
    }
}
