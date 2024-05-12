using System.Collections;
using UnityEngine;

public class Controller : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private Game _game;
    [SerializeField] private Board _board;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private UI _ui;
    [SerializeField] private SwipeDetection _swipeDetection;
    [SerializeField] private LevelReader _levelReader;
    [SerializeField] private Background _background;
    
    [SerializeField] private float _cycleLength = 1.5f;
    [SerializeField] private int _level = 1;
    
    [SerializeField] private float _moveAnimationTime;

    [SerializeField] private AudioSource _audioCountInBeat;
    [SerializeField] private AudioSource _audioSnare;
    [SerializeField] private AudioSource _audioRide;
    [SerializeField] private AudioSource _audioHiHat;

    private float time;
    private int numberOfCountInBeats;
    private bool waitingForMove;
    private bool onBeatHandled;
    private bool playerMoveHandled;
    private bool enemyMoveHandled;
    private bool gameEnded;
    private Direction nextMove = Direction.None;

    private void Start() {
        Application.targetFrameRate = 60;
        _levelReader.ReadLevelCsv("level"+_level);
        _ui.SetLevel("Level " + _level);
        PrepareGame();
        _game.Init(_levelReader.GetStartingPosition(), _levelReader.GetMaxFile(), _levelReader.GetMaxRank(), _levelReader.GetDisabledFields(), _levelReader.GetFlagRegion());
    }

    private void Update() {

        if (numberOfCountInBeats <= 4) {
            time = (time + Time.deltaTime);
            if (!(time > _cycleLength / 2f)) return;
            if (numberOfCountInBeats < 4) StartCoroutine(PlaySound(_audioCountInBeat));
            numberOfCountInBeats++;
            time = time % (_cycleLength / 2f);
        } else {
            time = (time + Time.deltaTime) % _cycleLength;
            // game loop
            if (time <= _cycleLength / 2f) {
                // On Beat - Wait for input
                if (gameEnded) {
                    
                    var playerWon = _game.GetWinningStatus();
                    if (playerWon) {
                        LoadNextLevel();
                    }
                    else {
                        ResetLevel();
                    }
                    StartCoroutine(PlaySound(_audioCountInBeat));
                    numberOfCountInBeats = 1;
                } else {
                    if (!onBeatHandled) {
                        StartCoroutine(PlaySound(_audioRide));
                        waitingForMove = true;
                        _game.ShowPossibleMoves();
                        _background.SetPrimary();
                        playerMoveHandled = false;
                        enemyMoveHandled = false;
                        onBeatHandled = true;
                    }
                    if (waitingForMove) { // user has not input move yet
                        _swipeDetection.enabled = true;
                    }
                }
            } else if (!gameEnded) {
                // Off Beat - Show moves
                if (time <= _cycleLength * .75f) { 
                    // show player move
                    if (!playerMoveHandled) {
                        _swipeDetection.DetectSwipeForCurrentTouch();
                        if (nextMove != Direction.None) {
                            StartCoroutine(PlaySound(_audioSnare));
                            gameEnded = _game.Move(nextMove);
                        } else {
                            StartCoroutine(PlaySound(_audioHiHat));
                        }
                        _background.SetSecondary();
                        _board.RemoveMoveIndicators();
                        nextMove = Direction.None;
                        _swipeDetection.enabled = false;
                    }
                    playerMoveHandled = true;
                } else if (!enemyMoveHandled) {
                    // showEnemyMove
                    var bestMove = _levelReader.GetBestMove(_game.GetPosition());
                    if(bestMove.from != bestMove.to) {
                        StartCoroutine(PlaySound(_audioSnare));
                        gameEnded = _game.MoveEnemy(bestMove.from, bestMove.to) | gameEnded;
                    } else {
                        StartCoroutine(PlaySound(_audioHiHat));
                    }
                    enemyMoveHandled = true;
                    onBeatHandled = false;
                    
                }
            }
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
        waitingForMove = false;
        _swipeDetection.enabled = false;
    }

    private void PrepareGame() {
        time = 0f;
        numberOfCountInBeats = 0;
        onBeatHandled = false;
        playerMoveHandled = false;
        enemyMoveHandled = false;
        waitingForMove = true;
        gameEnded = false;
        _background.SetPrimary();
    }

    private void ResetLevel() {
        _game.SetupLevel();
        PrepareGame();
    }

    private void LoadNextLevel() {
        _level++;
        _levelReader.ReadLevelCsv("level" + _level);
        _ui.SetLevel("Level " + _level);
        PrepareGame();
        _game.Init(_levelReader.GetStartingPosition(), _levelReader.GetMaxFile(), _levelReader.GetMaxRank(), _levelReader.GetDisabledFields(), _levelReader.GetFlagRegion());
    }

    private IEnumerator PlaySound(AudioSource sound) {
        yield return new WaitForSeconds(_moveAnimationTime);
        sound.Play();
    }

}
