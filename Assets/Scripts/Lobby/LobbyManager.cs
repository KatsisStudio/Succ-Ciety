using LewdieJam.Game;
using LewdieJam.Persistency;
using LewdieJam.SO;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace LewdieJam.Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        public static LobbyManager Instance { private set; get; }

        [SerializeField]
        private GameInfo _gameInfo;

        [SerializeField]
        private StatDisplay _healthStat, _atkPowerStat, _atkSpeedStat, _energyStat;

        [SerializeField]
        private TMP_Text _energy;

        [SerializeField]
        private TMP_Text _hornLevel;

        [SerializeField]
        private Button[] _buttonsPerHornLevels;

        [Header("Live2D")]
        [SerializeField]
        private Live2DManager _live2D;

        [Header("Debug")]
        [SerializeField]
        private bool _giveLotOfEnergy;

        public void LoadGame()
        {
            SceneManager.LoadScene("Main");
        }

        private void Awake()
        {
            Instance = this;

#if UNITY_EDITOR
            if (_giveLotOfEnergy)
            {
                PersistencyManager.Instance.SaveData.Energy += 100000;
            }
#endif

            _healthStat.Key = UpgradableStat.BaseHealth;
            _atkPowerStat.Key = UpgradableStat.AtkPower;
            _atkSpeedStat.Key = UpgradableStat.CharmPower;
            _energyStat.Key = UpgradableStat.EnergyGained;

            _live2D.SetHornLevel(Mathf.FloorToInt(Stat01 * (_gameInfo.HornLevels.Length - 1)));
            UpdateUI();
        }

        private void ToggleAttachment(Attachment attachment)
        {
            _live2D.ToggleAttachment(attachment);
            PersistencyManager.Instance.SaveData.Attachments = _live2D.Attachments;
            PersistencyManager.Instance.Save();
            UpdateUI();
        }
        public void ToggleLargeBreastsAttachment() => ToggleAttachment(Attachment.LargeBreasts);
        public void ToggleFutanariAttachment() => ToggleAttachment(Attachment.Futanari);
        public void TogglePregnantAttachment() => ToggleAttachment(Attachment.Pregnant);

        private float Stat01 => PersistencyManager.Instance.SaveData.Stats.Values.Sum() / ((float)Enum.GetValues(typeof(UpgradableStat)).Length * _gameInfo.MaxLevel);

        public void UpdateUI()
        {
            _energy.text = PersistencyManager.Instance.SaveData.Energy.ToString();
            var index = Mathf.FloorToInt(Stat01 * (_gameInfo.HornLevels.Length - 1));
            _live2D.SetHornLevel(index);
            _hornLevel.text = index.ToString();

            for (int i = 0; i < _buttonsPerHornLevels.Length; i++)
            {
                if (index > i * 2)
                {
                    _buttonsPerHornLevels[i].interactable = true;
                }
                else
                {
                    break;
                }
            }

            StatDisplay[] _allStats = new[]
            {
                _healthStat, _atkPowerStat, _atkSpeedStat, _energyStat
            };
            var delta = _gameInfo.MaxBuyCost - _gameInfo.MinBuyCost;
            foreach (var stat in _allStats)
            {
                stat.UpdateValue(PersistencyManager.Instance.SaveData.GetStatValue(stat.Key));

                if (stat.Level >= _gameInfo.MaxLevel)
                {
                    // We reached max level for this stat
                    stat.ToggleButton(false);
                }
                else
                {
                    // We get where we are in our cost curve
                    var val = _gameInfo.CostProgression.Evaluate(stat.Level / (float)_gameInfo.MaxLevel);
                    // Then do a cross product to get the actual price
                    var prod = Mathf.CeilToInt(val * delta + _gameInfo.MinBuyCost);
                    stat.ToggleButton(PersistencyManager.Instance.SaveData.Energy >= prod);
                    stat.Cost = prod;
                }
            }

            _live2D.SetDirty();
        }
    }
}
