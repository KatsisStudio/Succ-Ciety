using LewdieJam.SO;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        public void LoadGame()
        {
            SceneManager.LoadScene("Main");
        }

        private void Awake()
        {
            Instance = this;

            _healthStat.Key = UpgradableStat.BaseHealth.ToString();
            _atkPowerStat.Key = UpgradableStat.AtkPower.ToString();
            _atkSpeedStat.Key = UpgradableStat.AtkSpeed.ToString();
            _energyStat.Key = UpgradableStat.EnergyGained.ToString();

            UpdateUI();
        }

        public void UpdateUI()
        {
            _energy.text = PersistentData.Energy.ToString();
            _hornLevel.text = Mathf.FloorToInt(PersistentData.Stats.Values.Sum() / ((float)Enum.GetValues(typeof(UpgradableStat)).Length * _gameInfo.MaxLevel) * _gameInfo.MaxHornLevel).ToString();

            StatDisplay[] _allStats = new[]
            {
                _healthStat, _atkPowerStat, _atkSpeedStat, _energyStat
            };
            var delta = _gameInfo.MaxBuyCost - _gameInfo.MinBuyCost;
            foreach (var stat in _allStats)
            {
                stat.UpdateValue(PersistentData.GetStatValue(stat.Key));

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
                    stat.ToggleButton(PersistentData.Energy >= prod);
                    stat.Cost = prod;
                }
            }
        }
    }
}
