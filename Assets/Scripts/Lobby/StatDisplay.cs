using LewdieJam.Persistency;
using LewdieJam.SO;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LewdieJam.Lobby
{
    public class StatDisplay : MonoBehaviour
    {
        public UpgradableStat Key { get; set; }

        [SerializeField]
        private TMP_Text _label;

        private string _baseText;

        [SerializeField]
        private TMP_Text _value;

        public int Level { private set; get; }
        private int _cost;
        public int Cost
        {
            set
            {
                _cost = value;

                _label.text = $"{_baseText} ({_cost})";
            }
            get => _cost;
        }

        [SerializeField]
        private Button _add;

        private void Awake()
        {
            _baseText = _label.text;

            _add.onClick.AddListener(new(() =>
            {
                PersistencyManager.Instance.SaveData.Energy -= Cost;

                if (PersistencyManager.Instance.SaveData.Stats.ContainsKey(Key)) PersistencyManager.Instance.SaveData.Stats[Key]++;
                else PersistencyManager.Instance.SaveData.Stats.Add(Key, 1);

                PersistencyManager.Instance.Save();

                LobbyManager.Instance.UpdateUI();
            }));
        }

        public void UpdateValue(int value, int max)
        {
            Level = value;
            _value.text = value.ToString();

            if (value == max)
            {
                _label.text = $"{_baseText}";
            }
        }

        public void ToggleButton(bool value)
        {
            _add.interactable = value;
        }
    }
}
