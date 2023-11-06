using LewdieJam.Game;
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
        private TMP_Text _value;

        public int Level { private set; get; }
        public int Cost { set; get; }

        [SerializeField]
        private Button _add;

        private void Awake()
        {
            _add.onClick.AddListener(new(() =>
            {
                PersistentData.Energy -= Cost;

                if (PersistentData.Stats.ContainsKey(Key)) PersistentData.Stats[Key]++;
                else PersistentData.Stats.Add(Key, 1);

                LobbyManager.Instance.UpdateUI();
            }));
        }

        public void UpdateValue(int value)
        {
            Level = value;
            _value.text = value.ToString();
        }

        public void ToggleButton(bool value)
        {
            _add.interactable = value;
        }
    }
}
