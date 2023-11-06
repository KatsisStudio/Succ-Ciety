using LewdieJam.Game;
using LewdieJam.SO;
using TMPro;
using UnityEngine;

namespace LewdieJam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        [SerializeField]
        private GameInfo _info;
        public GameInfo Info => _info;

        [SerializeField]
        private TMP_Text _debugText;

        [SerializeField]
        private GameObject _hScene;

        public float GetStatValue(UpgradableStat stat, AnimationCurve curve, float maxVal)
        {
            return curve.Evaluate(PersistentData.GetStatValue(stat) / (float)_info.MaxLevel) * maxVal;
        }

        public bool CanPlay => !_hScene.activeInHierarchy;

        private int _energy;
        public int Energy
        {
            set
            {
                _energy = value;
                UpdateUI();
            }
            get => _energy;
        }

        private void Awake()
        {
            Instance = this;
            UpdateUI();
        }

        private void UpdateUI()
        {
            _debugText.text = $"Energy: {Energy}";
        }

        public void ToggleHScene(bool value)
        {
            _hScene.SetActive(value);
        }
    }
}
