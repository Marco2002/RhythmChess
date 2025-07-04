using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour {
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private Sprite _pauseImage, _resumeImage;
    [SerializeField] private LevelMenuUI _levelMenuUI;
    [SerializeField] private LevelBeatUI _levelBeatUI;
    [SerializeField] private SettingsPopupUI _settingsPopupUI;
    [SerializeField] private AnalyticsConsentUI _analyticsConsentUIPrefab;
    [SerializeField] private HowToPlayPopupUI _howToPlayPopupPrefab;
    public event Action OnPausePlayButtonClicked, OnLevelMenuOpened, OnLevelMenuClosed, OnLevelReset,
        OnSettingsPopupOpened, OnSettingsPopupClosed;
    public event Action<int> OnLevelSelected, OnCountInBeatsSettingsChanged;
    public event Action<bool> OnVibrationSettingChanged, OnSoundSettingChanged, OnAnalyticsConsentChanged;
    private Navbar navbar;
    private VisualElement root, pauseFlash;
    private Button pauseButton;
    private IconButton resetButton, hintButton;
    private HintCard hintCard;
    private int _level;
    public int NumberOfMoves {
        get => _numberOfMoves;
        set {
            _numberOfMoves = value;
            navbar.TitleText = "" + _numberOfMoves;
        }
    }
    private int _numberOfMoves;
    
    public int Level {
        set {
            navbar.LevelText = "LEVEL " + (value + 1);
            _levelMenuUI.ActiveLevel = value;
            _level = value;
        }
    }

    public bool TutorialMode {
        get => root.ClassListContains("game_tutorial-mode");
        set {
            if (value) {
                root.AddToClassList("game_tutorial-mode");
            } else {
                root.RemoveFromClassList("game_tutorial-mode");
            }
            _levelBeatUI.TutorialMode = value;
            navbar.TutorialMode = value;
        }
    }
    
    public void Pause() {
        root.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.3f));
        pauseButton.iconImage = Background.FromSprite(_resumeImage);
    }
    
    public void Resume() {
        navbar.Nodes = PlayerPrefs.GetInt("countInBeats");
        navbar.NodesEnabled = 0;
        root.style.backgroundColor = StyleKeyword.Null;
        pauseButton.iconImage = Background.FromSprite(_pauseImage);
    }

    public void ShowPauseFlash(bool pause) {
        StartCoroutine(PauseFlashAnimation(pause));
    }

    public void ResetProgress() {
        navbar.Nodes = PlayerPrefs.GetInt("countInBeats");
        navbar.NodesEnabled = 0;
        NumberOfMoves = 0;
        resetButton.SetEnabled(false);
    }
    public void StartOnBeat() {
        if(navbar.Nodes != 1) navbar.Nodes = 1;
        navbar.NodesEnabled = 0;
    }

    public void EnableResetButton() {
        resetButton.SetEnabled(true);
    }
    
    public void SetActiveNodes(int nodes) {
        navbar.NodesEnabled = nodes;
    }

    public void StartOffBeat() {
        navbar.NodesEnabled = 1;
    }

    public void OpenLevelBeatUI(int stars, int requiredStarsFor2, int requiredStarsFor3, int numberOfMoves) {
        _levelBeatUI.stars = stars;
        _levelBeatUI.RequiredStars = new[] { requiredStarsFor2, requiredStarsFor3 };
        _levelBeatUI.NumberOfMoves = numberOfMoves;
        root.style.visibility = Visibility.Hidden;
        _levelBeatUI.Open();
    }
    
    public void ShowHintCard(Sprite image, string hint) {
        hintCard.Label = hint;
        hintCard.Image = image;
        hintCard.AddToClassList("game-hint-card_active");
    }
    
    public void HideHintCard() {
        hintCard.RemoveFromClassList("game-hint-card_active");
    }
    
    private IEnumerator PauseFlashAnimation(bool pause) {
        pauseFlash.style.backgroundImage = Background.FromSprite(pause ? _pauseImage : _resumeImage);
        pauseFlash.style.display = DisplayStyle.Flex;

        const float duration = 0.5f;
        var elapsedTime = 0f;

        while (elapsedTime < duration) {
            pauseFlash.style.scale = new StyleScale(Vector3.Lerp(Vector3.one, Vector3.one * 1.5f, elapsedTime / duration));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        pauseFlash.style.display = DisplayStyle.None;
    }
    
    private void OpenAnalyticsConsentUI() {
        var analyticsConsentUI = Instantiate(_analyticsConsentUIPrefab, Vector3.zero, Quaternion.identity);
        analyticsConsentUI.Init();
        analyticsConsentUI.OnConsentDeclined += () => {
            _settingsPopupUI.Close();
            OnAnalyticsConsentChanged?.Invoke(false);
            Destroy(analyticsConsentUI.gameObject);
        };
        
        analyticsConsentUI.OnConsentAccepted += () => {
            _settingsPopupUI.Close();
            OnAnalyticsConsentChanged?.Invoke(true);
            Destroy(analyticsConsentUI.gameObject);
        };
    }
    
     public void Init(int level) {
        root = _uiDocument.rootVisualElement;
        _settingsPopupUI.Init();
        
        pauseButton = root.Q<Button>("PauseButton");
        navbar = root.Q<Navbar>("Navbar");
        pauseFlash = root.Q<VisualElement>("PauseFlash");
        resetButton = root.Q<IconButton>("ResetButton");
        hintButton = root.Q<IconButton>("HintButton");
        hintCard = root.Q<HintCard>("HintCard");

        Level = level;
        NumberOfMoves = 0;
        
        _levelMenuUI.OnCloseButtonClicked += () => OnLevelMenuClosed?.Invoke();
        
        _levelMenuUI.OnLevelButtonClicked += (levelNumber) => OnLevelSelected?.Invoke(levelNumber);
        
        _levelBeatUI.OnRetryButtonClicked += () => {
            OnLevelSelected?.Invoke(_level);
            _levelBeatUI.Close();
            root.style.visibility = Visibility.Visible;
        };

        _levelBeatUI.OnLevelMenuButtonClicked += () => {
            OnLevelMenuOpened?.Invoke();
            _levelBeatUI.Close();
            _levelMenuUI.Open();
            root.style.visibility = Visibility.Visible;
        };
        
        _levelBeatUI.OnNextLevelButtonClicked += () => {
            OnLevelSelected?.Invoke(_level + 1);
            _levelBeatUI.Close();
            root.style.visibility = Visibility.Visible;
        };
        
        _settingsPopupUI.OnCloseButtonClicked += () => {
            OnSettingsPopupClosed?.Invoke();
        };

        _settingsPopupUI.OnSoundSettingsChanged += (value) => {
            OnSoundSettingChanged?.Invoke(value);
        };
        _settingsPopupUI.OnVibrationSettingsChanged += (value) => {
            OnVibrationSettingChanged?.Invoke(value);
        };
        _settingsPopupUI.OnCountInBeatsSettingsChanged += (value) => {
            OnCountInBeatsSettingsChanged?.Invoke(value);
        };
        _settingsPopupUI.OnPrivacyPolicyClicked += OpenAnalyticsConsentUI;
        _settingsPopupUI.OnTutorialClicked += () => {
            _settingsPopupUI.Close();
            var htpPopup = Instantiate(_howToPlayPopupPrefab, Vector3.zero, Quaternion.identity);
            htpPopup.Init();
            htpPopup.Open();
            htpPopup.OnCloseButtonClicked += () => {
                Destroy(htpPopup.gameObject);
                OnLevelMenuClosed?.Invoke();
            };
        };
        
        navbar.OnLevelsButtonClicked += () => {
            OnLevelMenuOpened?.Invoke();
            _levelMenuUI.Open();
        };
        
        navbar.OnSettingsButtonClicked += () => {
            OnSettingsPopupOpened?.Invoke();
            _settingsPopupUI.Open();
        };
        navbar.Nodes = PlayerPrefs.GetInt("countInBeats");
        
        resetButton.SetEnabled(false);
        resetButton.OnClicked += () => OnLevelReset?.Invoke();
        
        if(PlayerPrefs.GetInt("analyticsConsent") == -1) {
            OpenAnalyticsConsentUI();
        }
        pauseButton.clicked += () => OnPausePlayButtonClicked?.Invoke();
     }
}