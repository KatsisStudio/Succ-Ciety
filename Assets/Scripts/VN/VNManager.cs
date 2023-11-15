using Ink.Runtime;
using LewdieJam.SO;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace LewdieJam.VN
{
    public class VNManager : MonoBehaviour
    {
        public static bool QuickRetry = false;
        public static VNManager Instance { private set; get; }

        [SerializeField]
        private TextDisplay _display;

        [SerializeField]
        private HSceneInfo _houseNotEnter;

        [SerializeField]
        private TextAsset _openDoorAsset;

        private string _currentCharacter;

        private Story _story;

        [SerializeField]
        private GameObject _container;

        [SerializeField]
        private GameObject _namePanel;

        [SerializeField]
        private TMP_Text _nameText;

        [SerializeField]
        private GameObject _openDoorQuestion;

        [SerializeField]
        private Image _hSceneVisual;

        private bool _isSkipEnabled;
        private float _skipTimer;
        private float _skipTimerRef = .1f;

        private Action _onDone;

        private HSceneInfo _currHScene;
        private int _backgroundIndex;

        private void Awake()
        {
            Instance = this;
        }

        public bool IsPlayingStory => _container.activeInHierarchy || _openDoorQuestion.activeInHierarchy;

        private void Update()
        {
            if (_isSkipEnabled)
            {
                _skipTimer -= Time.deltaTime;
                if (_skipTimer < 0)
                {
                    _skipTimer = _skipTimerRef;
                    DisplayNextDialogue();
                }
            }
        }

        public void ShowOpenDoorQuestion(HSceneInfo hScene)
        {
            _currHScene = hScene;
            _backgroundIndex = 0;
            ShowStory(_openDoorAsset, () =>
            {
                _openDoorQuestion.SetActive(true);
            });
        }

        public void ShowDoorRefuseStory()
        {
            _openDoorQuestion.SetActive(false);
            _currHScene = _houseNotEnter;
            _hSceneVisual.gameObject.SetActive(true);
            _hSceneVisual.sprite = _currHScene.Sprites[0];
            ShowStory(_currHScene.Story, null);
        }

        public void ShowHSceneStory(HSceneInfo hScene)
        {
            _currHScene = hScene;
            _backgroundIndex = 0;
            ShowHSceneStory();
        }

        public void ShowHSceneStory()
        {
            GameManager.Instance.EnableHSceneBgm();
            _openDoorQuestion.SetActive(false);
            _hSceneVisual.gameObject.SetActive(true);
            _hSceneVisual.sprite = _currHScene.Sprites[0];
            ShowStory(_currHScene.Story, () =>
            {
                GameManager.Instance.EnableGameBgm();
            });
        }

        public void ShowStory(TextAsset asset, Action onDone)
        {
            Debug.Log($"[STORY] Playing {asset.name}");
            _currentCharacter = string.Empty;
            _onDone = onDone;
            _story = new(asset.text);
            _isSkipEnabled = false;
            DisplayStory(_story.Continue());
        }

        private void DisplayStory(string text)
        {
            _container.SetActive(true);
            _namePanel.SetActive(false);

            foreach (var tag in _story.currentTags)
            {
                var s = tag.ToUpperInvariant().Split(' ');
                var content = string.Join(' ', s.Skip(1)).ToUpperInvariant();
                switch (s[0])
                {
                    case "SPEAKER":
                        if (content == "NONE") _currentCharacter = string.Empty;
                        else
                        {
                            _currentCharacter = content;
                        }
                        break;

                    case "BACKGROUND":
                        _backgroundIndex++;
                        _hSceneVisual.sprite = _currHScene.Sprites[_backgroundIndex];
                        break;

                    default:
                        Debug.LogError($"Unknown story key: {s[0]}");
                        break;
                }
            }
            _display.ToDisplay = text;
            if (string.IsNullOrEmpty(_currentCharacter))
            {
                _namePanel.SetActive(false);
            }
            else
            {
                _namePanel.SetActive(true);
                _nameText.text = _currentCharacter;
            }
        }

        public void DisplayNextDialogue()
        {
            if (!_container.activeInHierarchy)
            {
                return;
            }
            if (!_display.IsDisplayDone)
            {
                // We are slowly displaying a text, force the whole display
                _display.ForceDisplay();
            }
            else if (_story.canContinue && // There is text left to write
                !_story.currentChoices.Any()) // We are not currently in a choice
            {
                DisplayStory(_story.Continue());
            }
            else if (!_story.canContinue && !_story.currentChoices.Any())
            {
                _container.SetActive(false);
                _hSceneVisual.gameObject.SetActive(false);
                _onDone?.Invoke();
            }
        }

        public void ToggleSkip(bool value)
            => _isSkipEnabled = value;
    }
}