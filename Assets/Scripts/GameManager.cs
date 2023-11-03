using TMPro;
using UnityEngine;

namespace LewdieJam
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Instance { private set; get; }

        [SerializeField]
        private SO.GameInfo _info;

        [SerializeField]
        private TMP_Text _debugText;

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
    }
}
