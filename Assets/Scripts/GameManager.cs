using TMPro;
using UnityEngine;

namespace LewdieJam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        [SerializeField]
        private SO.GameInfo _info;
        public SO.GameInfo Info => _info;

        [SerializeField]
        private TMP_Text _debugText;

        [SerializeField]
        private GameObject _hScene;

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
