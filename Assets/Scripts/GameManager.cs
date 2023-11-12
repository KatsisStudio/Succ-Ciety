using LewdieJam.Game;
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

        public GameInfo Info => _info;

        [SerializeField]
        private TMP_Text _energyDisplay;

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

        private void Awake()
        {
            Instance = this;
            SceneManager.LoadScene("Map", LoadSceneMode.Additive);
            UpdateUI();
        }

        public void UpdateUI()
        {
            _energyDisplay.text = $"Energy: {PersistentData.Energy} ({PersistentData.PendingEnergy})";
        }
    }
}
