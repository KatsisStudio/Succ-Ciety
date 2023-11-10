using LewdieJam.Game;
using LewdieJam.SO;
using Live2D.Cubism.Core;
using System;
using System.Collections.Generic;
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

        [Header("Game Info SO")]
        [SerializeField]
        private GameInfo _gameInfo;

        [Header("UI")]
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
        private CubismParameter _breathParam;
        [SerializeField]
        private CubismParameter _breastsBounceParam;
        [SerializeField]
        private CubismParameter _breastsSizeModifierParam;
        [SerializeField]
        private CubismParameter _hornParam;

        [Header("Debug")]
        [SerializeField]
        private bool _giveLotOfEnergy;

        private bool _live2dDirty;

        private readonly List<UpdateTimer> _timers = new();

        private float _hornTimer;
        private float _hornTimerTarget;
        private int _hornIndex;

        public void OnBreastsClick()
        {
            if (!_timers.Any(x => x.IsId(_breastsBounceParam.Id)))
            {
                _timers.Add(new(_breastsBounceParam, 1.5f, false));
            }
        }

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
                PersistentData.Energy += 100000;
            }
#endif

            _healthStat.Key = UpgradableStat.BaseHealth;
            _atkPowerStat.Key = UpgradableStat.AtkPower;
            _atkSpeedStat.Key = UpgradableStat.CharmPower;
            _energyStat.Key = UpgradableStat.EnergyGained;

            _hornIndex = Mathf.FloorToInt(Stat01 * (_gameInfo.HornLevels.Length - 1));

            var tatooDelta = _hornParam.MaximumValue - _hornParam.MinimumValue;
            _hornTimerTarget = _gameInfo.HornLevels[_hornIndex];
            _hornParam.Value = _hornTimerTarget * _hornParam.MaximumValue + _hornParam.MinimumValue;

            UpdateUI();

            _timers.Add(new(_breathParam, .5f, true));
        }

        private void Update()
        {
            foreach (var timer in _timers)
            {
                timer.Update(Time.deltaTime);
            }

            _timers.RemoveAll(x => x.IsDeletionCandidate);

            if (_hornTimer < _hornTimerTarget)
            {
                _hornTimer += Time.deltaTime * .1f;
                if (_hornTimer > _hornTimerTarget)
                {
                    _hornTimer = _hornTimerTarget;
                }
                var tatooDelta = _hornParam.MaximumValue - _hornParam.MinimumValue;
                _hornParam.Value = _hornTimer * _hornParam.MaximumValue + _hornParam.MinimumValue;
            }

            // Somehow this doesn't work properly if done in UpdateUI (?)
            if (_live2dDirty)
            {
                _live2dDirty = false;
                _breastsSizeModifierParam.Value = PersistentData.Attachments.HasFlag(Attachment.LargeBreasts) ? _breastsSizeModifierParam.MaximumValue : _breastsSizeModifierParam.MinimumValue;
            }
        }

        private void ToggleAttachment(Attachment attachment)
        {
            if (PersistentData.Attachments.HasFlag(attachment))
            {
                PersistentData.Attachments &= ~attachment;
            }
            else
            {
                PersistentData.Attachments |= attachment;
            }
            UpdateUI();
        }
        public void ToggleLargeBreastsAttachment() => ToggleAttachment(Attachment.LargeBreasts);

        private float Stat01 => PersistentData.Stats.Values.Sum() / ((float)Enum.GetValues(typeof(UpgradableStat)).Length * _gameInfo.MaxLevel);

        public void UpdateUI()
        {
            _energy.text = PersistentData.Energy.ToString();
            _hornIndex = Mathf.FloorToInt(Stat01 * (_gameInfo.HornLevels.Length - 1));
            _hornTimerTarget = _gameInfo.HornLevels[_hornIndex];
            _hornLevel.text = _hornIndex.ToString();

            for (int i = 0; i < _buttonsPerHornLevels.Length; i++)
            {
                if (_hornIndex > i)
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

            _live2dDirty = true;
        }
    }

    class UpdateTimer
    {
        public UpdateTimer(CubismParameter cParam, float speedMultiplier, bool isPersistent)
        {
            _param = cParam;
            _speedMultiplier = speedMultiplier;
            _isPersistent = isPersistent;
        }

        public void Update(float deltaTime)
        {
            _timer += deltaTime * (_isGoingUp ? 1f : -1f) * _speedMultiplier;

            if (_isGoingUp && _timer >= _param.MaximumValue)
            {
                _timer = _param.MaximumValue;
                _isGoingUp = false;
            }
            else if (!_isGoingUp && _timer <= _param.MinimumValue)
            {
                _timer = _param.MinimumValue;
                _isGoingUp = true;
                if (!_isPersistent)
                {
                    _isDeletionCandidate = true;
                }
            }

            _param.Value = _timer;
        }

        public bool IsDeletionCandidate => _isDeletionCandidate;

        public bool IsId(string id) => _param.Id == id;

        private float _timer;
        private float _speedMultiplier;
        private bool _isGoingUp = true;
        private CubismParameter _param;
        private bool _isPersistent;
        private bool _isDeletionCandidate;
    }
}
