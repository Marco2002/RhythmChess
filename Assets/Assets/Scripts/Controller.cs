using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Controller : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private Game game;
    [SerializeField] private Board board;
    [SerializeField] private UI UI;
    [SerializeField] private SwipeDetection swipeDetection;
    [SerializeField] private CsvLevelReader levelReader;
    [SerializeField] private FenUtil fenUtil;
    [SerializeField] private float cycleLength = 1.5f;
    [SerializeField] private int level = 1;

    [SerializeField] private float moveAnimationTime = .1f;

    [SerializeField] private AudioSource audioCountInBeat;
    [SerializeField] private AudioSource audioSnare;
    [SerializeField] private AudioSource audioRide;
    [SerializeField] private AudioSource audioHihat;


    private Dictionary<string, ((int x, int y) from, (int x, int y) to)> moveMatrix;
    private float time = 0f;
    private int numOfCountInBeats = 0;
    private bool waitingForMove = false;
    private bool onBeatHandled = false;
    private bool playerMoveHandled = false;
    private bool enemyMoveHandled = false;
    private bool gameEnded = false;
    private Direction nextMove = Direction.None;

    void Start() {
        levelReader.ReadLevelCsv("level"+level);
        UI.setLevel("Level " + level);
        this.moveMatrix = levelReader.GetMoveMatrix();
        PrepareGame();
        game.Init(fenUtil.FenToPosition(levelReader.GetFen(), levelReader.GetMaxFile()), levelReader.GetMaxFile(), levelReader.GetMaxRank(), levelReader.GetDisabledFields(), levelReader.GetFlagRegion());
    }

    private void Update() {

        if (numOfCountInBeats <= 4) {
            time = (time + Time.deltaTime);
            if(time > cycleLength/2f) {
                if (numOfCountInBeats < 4) StartCoroutine(PlaySound(audioCountInBeat));
                numOfCountInBeats++;
                time = time % (cycleLength / 2f);

            }
        } else {
            time = (time + Time.deltaTime) % cycleLength;
            // game loop
            if (time <= cycleLength / 2f) {
                // On Beat - Wait for input
                if (gameEnded) {
                    
                    bool playerWon = game.GetWinningSatus();
                    if (playerWon) {
                        Debug.Log("Congratulations, you won!");
                        LoadNextLevel();
                    }
                    else {
                        ResetLevel();
                    }
                    StartCoroutine(PlaySound(audioCountInBeat));
                    numOfCountInBeats = 1;
                } else {
                    if (!onBeatHandled) {
                        StartCoroutine(PlaySound(audioRide));
                        waitingForMove = true;
                        game.ShowPossibleMoves();
                        Camera.main.backgroundColor = ColorScheme.primary;
                        playerMoveHandled = false;
                        enemyMoveHandled = false;
                        onBeatHandled = true;
                    }
                    if (waitingForMove) { // user has not input move yet
                        swipeDetection.enabled = true;
                    }
                }
            } else if (!gameEnded) {
                // Off Beat - Show moves
                if (time <= cycleLength * .75f) { 
                    // show player move
                    if (!playerMoveHandled) {
                        swipeDetection.DetectSwipeForCurrentTouch();
                        if (nextMove != Direction.None) {
                            StartCoroutine(PlaySound(audioSnare));
                            gameEnded = game.Move(nextMove);
                        } else {
                            StartCoroutine(PlaySound(audioHihat));
                        }
                        Camera.main.backgroundColor = ColorScheme.secondary;
                        board.RemoveMoveIndicators();
                        nextMove = Direction.None;
                        swipeDetection.enabled = false;
                    }
                    playerMoveHandled = true;
                } else if (!enemyMoveHandled) {
                    // showEnemyMove
                    string currentFen = fenUtil.PositionToFen(game.GetPosition());
                    ((int x, int y) from, (int x, int y) to) bestMove = moveMatrix.GetValueOrDefault(currentFen);
                    if(bestMove.from != bestMove.to) {
                        StartCoroutine(PlaySound(audioSnare));
                        gameEnded = game.MoveEnemy(bestMove.from, bestMove.to) | gameEnded;
                    } else {
                        StartCoroutine(PlaySound(audioHihat));
                    }
                    enemyMoveHandled = true;
                    onBeatHandled = false;
                    
                }
            }
        }

    }

    public void RecordMove(Vector2 direction) {
        List<Direction> possibleDirections = game.GetPossibleMoveDirections();
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
        board.RemoveMoveIndicators();
        waitingForMove = false;
        swipeDetection.enabled = false;
    }

    private void PrepareGame() {
        time = 0f;
        numOfCountInBeats = 0;
        onBeatHandled = false;
        playerMoveHandled = false;
        enemyMoveHandled = false;
        waitingForMove = true;
        gameEnded = false;
    }

    private void ResetLevel() {
        game.SetupLevel();
        PrepareGame();
    }

    private void LoadNextLevel() {
        level++;
        levelReader.ReadLevelCsv("level" + level);
        UI.setLevel("Level " + level);
        this.moveMatrix = levelReader.GetMoveMatrix();
        PrepareGame();
        game.Init(fenUtil.FenToPosition(levelReader.GetFen(), levelReader.GetMaxFile()), levelReader.GetMaxFile(), levelReader.GetMaxRank(), levelReader.GetDisabledFields(), levelReader.GetFlagRegion());
    }

    private IEnumerator PlaySound(AudioSource sound) {
        yield return new WaitForSeconds(moveAnimationTime);
        sound.Play();
    }

}
