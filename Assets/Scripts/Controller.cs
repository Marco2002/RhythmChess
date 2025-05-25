using System;
using System.Linq;
using System.Text;
using MoreMountains.Feedbacks;
using UnityEngine;

public class Controller : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private Game _game;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private SwipeDetection _swipeDetection;
    [SerializeField] private LevelReader _levelReader;
    [SerializeField] private BeatManager _beatManager;
    [SerializeField] private GameUI _gameUI;
    [SerializeField] private MMF_Player _hapticFeedback;
    [SerializeField] private int _level;
    [SerializeField] private AudioSource _audioBeatLevel;
    [SerializeField] private AudioListener _audioListener;
    
    private bool gameEnded;
    private Direction nextMove = Direction.None;
    private int numberOfMoves;
    private bool paused;
    private bool pauseAtNextOnBeat;
    private bool inOffBeat;
    private ((int x, int y) from, (int x, int y) to) nextEnemyMove;

    private void Start() {
        if(!PlayerPrefs.HasKey("playerPrefsInitialized")) {
            InitializePlayerPrefs();
        }
        Application.targetFrameRate = 120;
        _hapticFeedback.FeedbacksList[0].Active = PlayerPrefs.GetInt("vibrationEnabled") == 1;
        
        _audioListener.enabled = PlayerPrefs.GetInt("soundEnabled") == 1; 
        _level = PlayerPrefs.GetInt("currentLevel", _level);
        _levelReader.ReadLevelCsv("level"+(_level+1));

        _beatManager.OnCountInBeat += OnCountInBeat;
        _game.OnMoveIndicatorTapped += direction => {
            var directionVector = direction switch {
                Direction.Up => Vector2.up,
                Direction.Down => Vector2.down,
                Direction.Left => Vector2.left,
                Direction.Right => Vector2.right,
                Direction.UpRight => new Vector2(1, 1),
                Direction.DownRight => new Vector2(1, -1),
                Direction.UpLeft => new Vector2(-1, 1),
                Direction.DownLeft => new Vector2(-1, -1),
                _ => Vector2.zero
            };
            RecordMove(directionVector);
        };
        InitializeUI();
        PrepareGame();
        _game.Init(_levelReader.GetStartingPosition(), _levelReader.GetMaxFile(), _levelReader.GetMaxRank(), _levelReader.GetDisabledFields(), _levelReader.GetFlagRegion());
    }

    private void InitializeUI() {
        _gameUI.Init(_level);
        _gameUI.OnPausePlayButtonClicked += TogglePause;
        _gameUI.OnLevelMenuOpened += PauseLevel;
        _gameUI.OnLevelMenuClosed += ResetLevel;
        _gameUI.OnLevelSelected += LoadLevel;
        _gameUI.OnLevelReset += ResetLevel;
        _gameUI.OnSettingsPopupOpened += PauseLevel;
        _gameUI.OnSettingsPopupClosed += ResumeLevel;
        _gameUI.OnVibrationSettingChanged += (value) => {
            _hapticFeedback.FeedbacksList[0].Active = value;
        };
        _gameUI.OnSoundSettingChanged += (value) => {
            _audioListener.enabled = value;
        };
    }

    private void HandleGameEnd(bool won = false) {
        var playerWon = won || _game.GetWinningStatus();
        if (playerWon) {
            BeatLevel();
        } else {
            ResetLevel();
        }
    }

    private void OnCountInBeat(int beat) {
        _hapticFeedback.PlayFeedbacks();
        _gameUI.SetActiveNodes(beat);
    }
    public void HandleOffBeat() {
        inOffBeat = false;
        if(pauseAtNextOnBeat) {
            pauseAtNextOnBeat = false;
            PauseLevel();
            return;
        }
        if (gameEnded) {
            HandleGameEnd();
            return;
        }
        _game.ShowPossibleMoves();
        _gameUI.NumberOfMoves++;
    }

    public void HandleOnBeat() {
        numberOfMoves++;
        inOffBeat = true;
        _gameUI.EnableResetButton();
        // show player move
        _swipeDetection.DetectSwipeForCurrentTouch();
        if (nextMove != Direction.None) {
            _game.Move(nextMove);
        }
        nextEnemyMove = _levelReader.GetBestMove(_game.GetPosition());
        _game.RemoveMoveIndicators();
        nextMove = Direction.None;
        _swipeDetection.enabled = false;
        
        if (nextEnemyMove == LevelReader.INVALID_MOVE) {
            HandleGameEnd(true);
        }
    }

    public void MoveEnemy() {
        if(nextEnemyMove.from != nextEnemyMove.to) {
            gameEnded = _game.MoveEnemy(nextEnemyMove.from, nextEnemyMove.to) | gameEnded;
        }
        _swipeDetection.enabled = true;
    }

    public void RecordMove(Vector2 direction) {
        var possibleDirections = _game.GetPossibleMoveDirections();
        if (Vector2.Dot(new Vector2(1, 1).normalized, direction) > .92 && possibleDirections.Contains(Direction.UpRight)) {
            nextMove = Direction.UpRight;
        }
        else if (Vector2.Dot(new Vector2(1, -1).normalized, direction) > .92 && possibleDirections.Contains(Direction.DownRight)) {
            nextMove = Direction.DownRight;
        }
        else if (Vector2.Dot(new Vector2(-1, 1).normalized, direction) > .92 && possibleDirections.Contains(Direction.UpLeft)) {
            nextMove = Direction.UpLeft;
        }
        else if (Vector2.Dot(new Vector2(-1, -1).normalized, direction) > .92 && possibleDirections.Contains(Direction.DownLeft)) {
            nextMove = Direction.DownLeft;
        }
        else if (Vector2.Dot(Vector2.up, direction) > .7 && possibleDirections.Contains(Direction.Up)) {
            nextMove = Direction.Up;
        }
        else if (Vector2.Dot(Vector2.down, direction) > .7 && possibleDirections.Contains(Direction.Down)) {
            nextMove = Direction.Down;
        }
        else if (Vector2.Dot(Vector2.left, direction) > .7 && possibleDirections.Contains(Direction.Left)) {
            nextMove = Direction.Left;
        }
        else if (Vector2.Dot(Vector2.right, direction) > .7 && possibleDirections.Contains(Direction.Right)) {
            nextMove = Direction.Right;
        }
        _game.RemoveMoveIndicators();
        _swipeDetection.enabled = false;
    }

    private void PrepareGame() {
        gameEnded = false;
        _gameUI.ResetProgress();
        ResumeLevel();
        numberOfMoves = 0;
    }

    private void ResetLevel() {
        _beatManager.Pause();
        _game.SetupLevel();
        _game.RemoveMoveIndicators();
        PrepareGame();
    }

    private void TogglePause() {
        _gameUI.ShowPauseFlash(!paused);
        if (paused) {
            ResumeLevel();
        } else {
            if (inOffBeat) {
                pauseAtNextOnBeat = true;
            } else {
                PauseLevel();
            }
        }
    }

    private void PauseLevel() {
        paused = true;
        _beatManager.Pause();
        _game.RemoveMoveIndicators();
        _gameUI.Pause();
    }

    private void ResumeLevel() {
        if (gameEnded) {
            HandleGameEnd();
            return;
        }
        paused = false;
        inOffBeat = false;
        _beatManager.Play();
        _gameUI.Resume();
    }

    private void BeatLevel() {
        var levelStatus = PlayerPrefs.GetString("levelStatus");
        var stars = 1;
        var requiredStarsFor3 = _levelReader.GetSolution().Count;
        var requiredStarsFor2 = Math.Max(
            _levelReader.GetSolution().Count + 1,
            (int) Math.Floor(_levelReader.GetSolution().Count * 1.3));
        if (numberOfMoves <= requiredStarsFor2) stars = 2;
        if (numberOfMoves <= requiredStarsFor3) stars = 3;
        if(levelStatus[_level] - '0' < stars) {
            var levelStatusString = new StringBuilder(levelStatus) {
                [_level] = stars.ToString()[0]
            };
            PlayerPrefs.SetString("levelStatus", levelStatusString.ToString());
            PlayerPrefs.Save();
        }
        _level++;
        if (_level > PlayerPrefs.GetInt("currentLevel")) {
            PlayerPrefs.SetInt("currentLevel", _level);
            PlayerPrefs.Save();
        }
        _beatManager.Pause();
        _gameUI.OpenLevelBeatUI(stars, requiredStarsFor2, requiredStarsFor3, numberOfMoves);
        _audioBeatLevel.Play();
    }

    private void LoadLevel(int level) {
        _level = level;
        _levelReader.ReadLevelCsv("level" + (_level + 1));
        _gameUI.Level = _level;
        PrepareGame();
        _game.Init(_levelReader.GetStartingPosition(), _levelReader.GetMaxFile(), _levelReader.GetMaxRank(), _levelReader.GetDisabledFields(), _levelReader.GetFlagRegion());
    }

    private void InitializePlayerPrefs() {
        var defaultStatus = string.Concat(Enumerable.Repeat(0.ToString(), GameConstants.NUMBER_OF_LEVELS));
        PlayerPrefs.SetString("levelStatus", PlayerPrefs.GetString("levelStatus", defaultStatus));
        
        PlayerPrefs.SetInt("currentLevel", 0);
        PlayerPrefs.SetInt("soundEnabled", 1);
        PlayerPrefs.SetInt("vibrationEnabled", 1);
        PlayerPrefs.SetInt("countInBeats", 4);
        PlayerPrefs.SetInt("playerPrefsInitialized", 1);
        PlayerPrefs.Save();
    }
}
