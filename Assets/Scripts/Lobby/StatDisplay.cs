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
        private TMP_Text _value;

        public int Level { private set; get; }
        public int Cost { set; get; }

        [SerializeField]
        private Button _add;

        private void Awake()
        {
            _add.onClick.AddListener(new(() =>
            {
                PersistencyManager.Instance.SaveData.Energy -= Cost;

                if (PersistencyManager.Instance.SaveData.Stats.ContainsKey(Key)) PersistencyManager.Instance.SaveData.Stats[Key]++;
                else PersistencyManager.Instance.SaveData.Stats.Add(Key, 1);

                PersistencyManager.Instance.Save();

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
