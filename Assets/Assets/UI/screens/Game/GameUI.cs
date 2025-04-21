using System;
using UnityEngine;
using System.Collections;
using UnityEngine.UIElements;

public class GameUI : MonoBehaviour {
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private Sprite _pauseImage, _resumeImage;
    [SerializeField] private LevelMenuUI _levelMenuUI;
    public event Action OnPausePlayButtonClicked, OnLevelMenuOpened, OnLevelMenuClosed, OnLevelReset;
    public event Action<int> OnLevelSelected;
    private Navbar navbar;
    private VisualElement root, pauseFlash;
    private Button pauseButton;
    private Coroutine runningProgressAnimation;
    private IconButton resetButton, hintButton;
    private int NumberOfMoves {
        get => _numberOfMoves;
        set {
            _numberOfMoves = value;
            navbar.TitleText = "" + _numberOfMoves;
        }
    }
    private int _numberOfMoves;

    public void UpdateLevel(int level) {
        navbar.LevelText = "LEVEL " + level;
    }
    
    public void Pause() {
        root.style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.3f));
        pauseButton.iconImage = Background.FromSprite(_resumeImage);
        PauseProgress();
    }

    private void PauseProgress() {
        if (runningProgressAnimation != null) {
            StopCoroutine(runningProgressAnimation);
            runningProgressAnimation = null;
        }
    }
    
    public void Resume() {
        root.style.backgroundColor = StyleKeyword.Null;
        pauseButton.iconImage = Background.FromSprite(_pauseImage);
        navbar.Progress = 0;
    }

    public void ShowPauseFlash(bool pause) {
        StartCoroutine(PauseFlashAnimation(pause));
    }

    public void ResetProgress() {
        IEnumerator SetProgressToZero() {
            yield return null; // Wait for the next frame
            navbar.Progress = 0;
        }

        NumberOfMoves = 0;
        resetButton.SetEnabled(false);
        PauseProgress();
        StartCoroutine(SetProgressToZero());
    }
    public void StartOnBeat() {
        runningProgressAnimation = StartCoroutine(ProgressAnimation(0, 50, 60f / 80f));
    }

    public void StartOffBeat() {
        NumberOfMoves++;
        resetButton.SetEnabled(true);
        runningProgressAnimation = StartCoroutine(ProgressAnimation(50, 100, 60f / 80f));
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
    
    private IEnumerator ProgressAnimation(float from, float to, float duration) {
        var elapsedTime = 0f;

        while (elapsedTime < duration) {
            navbar.Progress = Mathf.Lerp(from, to, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }
    
     public void Init(int level) {
        root = _uiDocument.rootVisualElement;
        
        pauseButton = root.Q<Button>("PauseButton");
        navbar = root.Q<Navbar>("Navbar");
        pauseFlash = root.Q<VisualElement>("PauseFlash");
        resetButton = root.Q<IconButton>("ResetButton");
        hintButton = root.Q<IconButton>("HintButton");

        NumberOfMoves = 0;
        _levelMenuUI.OnCloseButtonClicked += () => OnLevelMenuClosed?.Invoke();
        
        _levelMenuUI.OnLevelButtonClicked += (levelNumber) => OnLevelSelected?.Invoke(levelNumber);
        
        navbar.OnLevelsButtonClicked += () => {
            OnLevelMenuOpened?.Invoke();
            _levelMenuUI.Open();
        };
        
        resetButton.SetEnabled(false);
        resetButton.OnClicked += () => OnLevelReset?.Invoke();
        
        // navbar.OnSettingsButtonClicked += () => OnSettingsButtonClicked.Invoke();
        pauseButton.clicked += () => OnPausePlayButtonClicked?.Invoke();
        
        UpdateLevel(level);
     }
}