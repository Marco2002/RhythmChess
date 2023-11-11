using System.Collections.Generic;
using UnityEngine;

public class Controller : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private Game game;
    [SerializeField] private Board board;
    [SerializeField] private SwipeDetection swipeDetection;
    [SerializeField] private CsvLevelReader levelReader;
    [SerializeField] private FenUtil fenUtil;
    [SerializeField] private float cycleLength = 1.5f;
    [SerializeField] private int level = 1;

    [SerializeField] private AudioSource audioCountIn;
    [SerializeField] private AudioSource audioTrack;
    

    private Dictionary<string, ((int x, int y) from, (int x, int y) to)> moveMatrix;
    private float time = 0f;
    private bool waitingForMove = false;
    private bool firstFireInOnBeat = true;
    private bool firstFireInOffBeat = true;
    private bool firstTimeCountIn = true;
    private bool countInDone = false;
    private bool gameEnded = false;
    private bool pastFirstUpdate = false;// for some reason the first update is off the time for the rest of the game and needs to be skipped
    private Direction nextMove = Direction.None;

    void Start() {
        levelReader.ReadLevelCsv("level"+level);
        this.moveMatrix = levelReader.GetMoveMatrix();
        PrepareGame();
        game.Init(fenUtil.FenToPosition(levelReader.GetFen(), levelReader.GetMaxFile()), levelReader.GetMaxFile(), levelReader.GetMaxRank(), levelReader.GetDisabledFields(), levelReader.GetFlagRegion());
    }

    private void Update() {

        if(!pastFirstUpdate) {
            pastFirstUpdate = true;
            return;
        }
        if (!countInDone) {
            if(firstTimeCountIn) {
                // count in
                audioCountIn.Play();
                Invoke(nameof(StartGame), 2f * cycleLength);
                firstTimeCountIn = false;
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
                }
                else {
                    if (firstFireInOnBeat) {
                        waitingForMove = true;
                        game.ShowPossibleMoves();
                        Camera.main.backgroundColor = ColorScheme.primary;
                        firstFireInOffBeat = true;
                        firstFireInOnBeat = false;
                    }
                    if (waitingForMove) { // user has not input move yet
                        swipeDetection.enabled = true;
                    }
                }
            } else if(!gameEnded) {
                
                // Off Beat - Show moves
                if (firstFireInOffBeat) { // first time firing Off Beat
                    if (nextMove != Direction.None) {
                        gameEnded = game.Move(nextMove);
                    }
                    if (!gameEnded) {
                        string currentFen = fenUtil.PositionToFen(game.GetPosition());
                        ((int x, int y) from, (int x, int y) to) bestMove = moveMatrix.GetValueOrDefault(currentFen);
                        gameEnded = game.MoveEnemy(bestMove.from, bestMove.to) | gameEnded;
                    }
                    Camera.main.backgroundColor = ColorScheme.secondary;
                    board.RemoveMoveIndicators();
                    nextMove = Direction.None;
                    swipeDetection.enabled = false;

                    
                    firstFireInOnBeat = true;
                    firstFireInOffBeat = false;
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
        firstFireInOnBeat = true;
        firstFireInOffBeat = true;
        firstTimeCountIn = true;
        countInDone = false;
        waitingForMove = true;
        gameEnded = false;
        audioTrack.Stop();

    }
    private void StartGame() {
        audioTrack.Play();
        countInDone = true;
    }

    private void ResetLevel() {
        game.SetupLevel();
        PrepareGame();
    }

    private void LoadNextLevel() {
        level++;
        levelReader.ReadLevelCsv("level" + level);
        this.moveMatrix = levelReader.GetMoveMatrix();
        PrepareGame();
        game.Init(fenUtil.FenToPosition(levelReader.GetFen(), levelReader.GetMaxFile()), levelReader.GetMaxFile(), levelReader.GetMaxRank(), levelReader.GetDisabledFields(), levelReader.GetFlagRegion());
    }

}
