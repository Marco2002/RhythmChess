using System.Linq;
using System.Text;
using UnityEngine;

public class Controller : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private Game _game;
    [SerializeField] private Board _board;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private SwipeDetection _swipeDetection;
    [SerializeField] private LevelReader _levelReader;
    [SerializeField] private BeatManager _beatManager;
    [SerializeField] private GameUI _gameUI;
    
    [SerializeField] private int _level = 0;
    
    private bool gameEnded;
    private Direction nextMove = Direction.None;
    private int numberOfMoves;
    private bool paused;
    private bool pauseAtNextOnBeat;
    private bool inOffBeat;
    private bool playerPrefsInitialized;

    private void Start() {
        if (!playerPrefsInitialized) {
            InitializePlayerPrefs();
            playerPrefsInitialized = true;
        }
        Application.targetFrameRate = 120;
        _level = PlayerPrefs.GetInt("currentLevel", _level);
        _levelReader.ReadLevelCsv("level"+(_level+1));
        InitializeUI();
        PrepareGame();
        _game.Init(_levelReader.GetStartingPosition(), _levelReader.GetMaxFile(), _levelReader.GetMaxRank(), _levelReader.GetDisabledFields(), _levelReader.GetFlagRegion());
    }

    private void InitializeUI() {
        _gameUI.Init(_level + 1);
        _gameUI.OnPausePlayButtonClicked += TogglePause;
        _gameUI.OnLevelMenuOpened += PauseLevel;
        _gameUI.OnLevelMenuClosed += PrepareGame;
        _gameUI.OnLevelSelected += LoadLevel;
        _gameUI.OnLevelReset += ResetLevel;
    }

    private void HandleGameEnd() {
        var playerWon = _game.GetWinningStatus();
        if (playerWon) {
            BeatLevel();
        } else {
            ResetLevel();
        }
    }

    public void HandleOnBeat() {
        inOffBeat = false;
        if(pauseAtNextOnBeat) {
            pauseAtNextOnBeat = false;
            PauseLevel();
            return;
        }
        if (gameEnded) {
            HandleGameEnd();
        } else {
            _game.ShowPossibleMoves();
            _swipeDetection.enabled = true;
        }
    }

    public void HandeOffBeat() {
        numberOfMoves++;
        inOffBeat = true;
        // show player move
        _swipeDetection.DetectSwipeForCurrentTouch();
        if (nextMove != Direction.None) {
            gameEnded = _game.Move(nextMove);
        }
        _board.RemoveMoveIndicators();
        nextMove = Direction.None;
        _swipeDetection.enabled = false;
    }

    public void MoveEnemy() {
        // showEnemyMove
        var bestMove = _levelReader.GetBestMove(_game.GetPosition());
        if(bestMove.from != bestMove.to) {
            gameEnded = _game.MoveEnemy(bestMove.from, bestMove.to) | gameEnded;
        }
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
        _board.RemoveMoveIndicators();
        _swipeDetection.enabled = false;
    }

    private void PrepareGame() {
        gameEnded = false;
        _gameUI.ResetProgress();
        ResumeLevel();
        numberOfMoves = 0;
    }

    private void ResetLevel() {
        _game.SetupLevel();
        PrepareGame();
    }
    
    public void TogglePause() {
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

    public void PauseLevel() {
        paused = true;
        _beatManager.Stop();
        _beatManager.enabled = false;
        _board.RemoveMoveIndicators();
        _gameUI.Pause();
    }
    
    public void ResumeLevel() {
        if (gameEnded) {
            HandleGameEnd();
            return;
        }
        paused = false;
        inOffBeat = false;
        _beatManager.enabled = true;
        _beatManager.Reset();
        _gameUI.Resume();
    }

    private void BeatLevel() {
        var levelStatus = PlayerPrefs.GetString("levelStatus");
        Debug.Log("level status "+levelStatus);
        var stars = 1;
        if (numberOfMoves <= _levelReader.GetSolution().Count * 1.5) stars = 2;
        if (numberOfMoves == _levelReader.GetSolution().Count) stars = 3;
        if(levelStatus[_level] - '0' < stars) {
            var levelStatusString = new StringBuilder(levelStatus);
            levelStatusString[_level] = stars.ToString()[0];
            PlayerPrefs.SetString("levelStatus", levelStatusString.ToString());
            PlayerPrefs.Save();
        }
        _level++;
        if (_level > PlayerPrefs.GetInt("currentLevel")) {
            PlayerPrefs.SetInt("currentLevel", _level);
            PlayerPrefs.Save();
        }
        LoadLevel(_level);
    }
    
    public void LoadLevel(int level) {
        _level = level;
        _levelReader.ReadLevelCsv("level" + (_level + 1));
        _gameUI.UpdateLevel(_level + 1);
        PrepareGame();
        _game.Init(_levelReader.GetStartingPosition(), _levelReader.GetMaxFile(), _levelReader.GetMaxRank(), _levelReader.GetDisabledFields(), _levelReader.GetFlagRegion());
    }

    private void InitializePlayerPrefs() {
        if(!PlayerPrefs.HasKey("levelStatus")) {
            var defaultStatus = string.Concat(Enumerable.Repeat(0.ToString(), GameConstants.NUMBER_OF_LEVELS));
            PlayerPrefs.SetString("levelStatus", PlayerPrefs.GetString("levelStatus", defaultStatus));
        }
        if(!PlayerPrefs.HasKey("currentLevel")) {
            PlayerPrefs.SetInt("currentLevel", 0);
        }
        PlayerPrefs.Save();
    }
}
