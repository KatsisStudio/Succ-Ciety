using LewdieJam.Game;
using LewdieJam.SO;
using Live2D.Cubism.Core;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LewdieJam.Lobby
{
    public class Live2DManager : MonoBehaviour
    {
        [SerializeField]
        private GameInfo _gameInfo;

        [SerializeField]
        private CubismParameter _breathParam;
        [SerializeField]
        private CubismParameter _breastsBounceParam;
        [SerializeField]
        private CubismParameter _breastsSizeModifierParam;
        [SerializeField]
        private CubismParameter _hornParam;

        private readonly List<UpdateTimer> _timers = new();

        private float _hornTimerTarget;

        private bool _live2dDirty;

        private float _hornTimer;
        private int _hornIndex;

        public Attachment Attachments { private set; get; }

        public void ToggleAttachment(Attachment attachment)
        {
            if (Attachments.HasFlag(attachment))
            {
                Attachments &= ~attachment;
            }
            else
            {
                Attachments |= attachment;
            }
        }

        private void Awake()
        {
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

        public void SetHornLevel(int index)
        {
            _hornIndex = index;
            _hornTimerTarget = _gameInfo.HornLevels[_hornIndex];
            _hornParam.Value = _hornTimerTarget * _hornParam.MaximumValue + _hornParam.MinimumValue;
        }

        public void OnBreastsClick()
        {
            if (!_timers.Any(x => x.IsId(_breastsBounceParam.Id)))
            {
                _timers.Add(new(_breastsBounceParam, 1.5f, false));
            }
        }

        public void SetDirty()
        {
            _live2dDirty = true;
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
}
