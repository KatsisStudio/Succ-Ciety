using Ink.Runtime;
using LewdieJam.Achievement;
using LewdieJam.SO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
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
        private TextAsset _hSceneEnd;

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
        private GameObject _openDoorQuestion, _endQuestion;

        [SerializeField]
        private Image _hSceneVisual;

        [SerializeField]
        private AudioSource _hSceneBgm;

        [SerializeField]
        private AudioSource _moansPlayer;

        [SerializeField]
        private List<AudioClip> _moans;

        [SerializeField]
        private Transform _choiceContainer;

        [SerializeField]
        private GameObject _choicePrefab;

        private bool _isPlayingMoans;

        private bool _isSkipEnabled;
        private float _skipTimer;
        private float _skipTimerRef = .1f;

        private Action _onDone;

        private HSceneInfo _currHScene;
        private int _backgroundIndex;

        private Action _hSceneEndCallback;

        private void Awake()
        {
            Instance = this;
            SceneManager.LoadScene("AchievementManager", LoadSceneMode.Additive);

            _display.OnDisplayDone += (_sender, _e) =>
            {
                if (_story.currentChoices.Any())
                {
                    foreach (var choice in _story.currentChoices)
                    {
                        var button = Instantiate(_choicePrefab, _choiceContainer);
                        button.GetComponentInChildren<TMP_Text>().text = choice.text;

                        var elem = choice;
                        button.GetComponent<Button>().onClick.AddListener(() =>
                        {
                            _story.ChoosePath(elem.targetPath);
                            for (int i = 0; i < _choiceContainer.childCount; i++)
                                Destroy(_choiceContainer.GetChild(i).gameObject);
                            DisplayStory(_story.Continue());
                        });
                    }
                }
            };
        }

        public bool IsPlayingStory => _container.activeInHierarchy || _openDoorQuestion.activeInHierarchy || _endQuestion.activeInHierarchy;

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

        public void InitHScene(HSceneInfo hScene)
        {
            _currHScene = hScene;
            _backgroundIndex = 0;
        }

        public void ShowOpenDoorQuestion(HSceneInfo hScene)
        {
            InitHScene(hScene);
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
            ShowStory(_currHScene.Story, () =>
            {
                AchievementManager.Instance.Unlock(AchievementID.DontEnterScene);
            });
        }

        /// <summary>
        /// Not really pretty but we use this method to tell the game manager we need to load an H scene
        /// </summary>
        public void LoadHSceneGameManagerBridge()
        {
            GameManager.Instance.PlayHScene();
        }

        /// <summary>
        /// Used by the gallery to play a specific H Scene
        /// </summary>
        /// <param name="hScene"></param>
        public void ShowHSceneStory(HSceneInfo hScene)
        {
            InitHScene(hScene);
            ShowHSceneStoryWithCallback(null, false);
        }

        /// <summary>
        /// Use to start a H Scene
        /// </summary>
        public void ShowHSceneStoryWithCallback(Action onDone, bool showChoiceAtEnd)
        {
            _hSceneBgm.Play();
            _openDoorQuestion.SetActive(false);
            _hSceneVisual.gameObject.SetActive(true);
            _hSceneVisual.sprite = _currHScene.Sprites[0];
            _hSceneEndCallback = () =>
            {
                _hSceneBgm.Stop();
                onDone?.Invoke();
            };
            if (showChoiceAtEnd)
            {
                ShowStory(_currHScene.Story, () =>
                {
                    AchievementManager.Instance.Unlock(_currHScene.Achievement);
                    ShowStory(_hSceneEnd, () =>
                    {
                        _endQuestion.SetActive(true);
                    });
                });
            }
            else
            {
                ShowStory(_currHScene.Story, _hSceneEndCallback);
            }
        }

        public void LoadLobby()
        {
            SceneManager.LoadScene("Lobby");
        }

        public void Continue()
        {
            _hSceneEndCallback();
            _endQuestion.SetActive(false);
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

                    case "MOANS":
                        if (content == "ON")
                        {
                            if (_isPlayingMoans)
                            {
                                Debug.LogWarning("Moans are already playing, directive ignored");
                                break;
                            }

                            _isPlayingMoans = true;
                            StartCoroutine(PlayNextMoan());
                        }
                        else if (content == "OFF")
                        {
                            _isPlayingMoans = false;
                            _moansPlayer.Stop();
                        }
                        else
                        {
                            Debug.LogError($"Invalid moans value {content}");
                        }
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

        public IEnumerator PlayNextMoan()
        {
            while (_isPlayingMoans)
            {
                var index = UnityEngine.Random.Range(0, _moans.Count);
                _moansPlayer.clip = _moans[index];
                _moans.RemoveAt(index);
                _moans.Insert(0, _moansPlayer.clip);
                _moansPlayer.Play();

                yield return new WaitForSeconds(_moansPlayer.clip.length);
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
            else if (_story.canContinue && !_story.currentChoices.Any()) // There is text left to write
            {
                DisplayStory(_story.Continue());
            }
            else if (!_story.canContinue && !_story.currentChoices.Any())
            {
                _isPlayingMoans = false;
                _moansPlayer.Stop();
                _container.SetActive(false);
                _hSceneVisual.gameObject.SetActive(false);
                _onDone?.Invoke();
            }
        }

        public void ToggleSkip(bool value)
            => _isSkipEnabled = value;
    }
}