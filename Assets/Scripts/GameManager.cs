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
        private TMP_Text _energyDisplay;

        [SerializeField]
        private GameObject _hScene;

        public float GetStatValue(UpgradableStat stat, AnimationCurve curve, float maxVal)
        {
            return curve.Evaluate(PersistentData.GetStatValue(stat) / (float)_info.MaxLevel) * maxVal;
        }

        public bool CanPlay => !_hScene.activeInHierarchy;

        private void Awake()
        {
            Instance = this;
            UpdateUI();
        }

        public void UpdateUI()
        {
            _energyDisplay.text = $"Energy: {PersistentData.Energy} ({PersistentData.PendingEnergy})";
        }

        public void ToggleHScene(bool value)
        {
            _hScene.SetActive(value);
        }
    }
}
