using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;

public class Controller : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private Game game;
    [SerializeField] private Board board;
    [SerializeField] private SwipeDetection swipeDetection;
    [SerializeField] private CsvLevelReader levelReader;
    [SerializeField] private FenUtil fenUtil;
    [SerializeField] private float cycleLength = 1.5f;
    

    private Dictionary<string, ((int x, int y) from, (int x, int y) to)> moveMatrix;
    private float time = 0f;
    private bool waitingForMove = false;
    private bool firstFireInOnBeat = true;
    private bool firstFireInOffBeat = true;
    private Direction nextMove = Direction.None;

    void Start() {
        levelReader.ReadLevelCsv("level1");
        this.moveMatrix = levelReader.GetMoveMatrix();
        game.Init(fenUtil.FenToPosition(levelReader.GetFen(), levelReader.GetMaxFile()), levelReader.GetMaxFile(), levelReader.GetMaxRank(), levelReader.GetDisabledFields(), levelReader.GetFlagRegion());
        waitingForMove = true;
    }

    private void Update() {
        time = (time + Time.deltaTime) % cycleLength; // repeat every game cycle
        if (time <= cycleLength/2f) {
            // On Beat - Wait for input
            if(firstFireInOnBeat) {
                waitingForMove = true;
                game.ShowPossibleMoves();
                Camera.main.backgroundColor = ColorScheme.primary;
                firstFireInOffBeat = true;
                firstFireInOnBeat = false;
            }
            if(waitingForMove) { // user has not input move yet
                swipeDetection.enabled = true;
            }
            
        } else {
            // Off Beat - Show moves
            if (firstFireInOffBeat) { // first time firing Off Beat
                if(nextMove != Direction.None) {
                    game.Move(nextMove);
                }
                string currentFen = fenUtil.PositionToFen(game.GetPosition());
                ((int x, int y) from, (int x, int y) to) bestMove = moveMatrix.GetValueOrDefault(currentFen);
                game.MoveEnemy(bestMove.from, bestMove.to);

                Camera.main.backgroundColor = ColorScheme.secondary;
                board.RemoveMoveIndicators();
                nextMove = Direction.None;
                swipeDetection.enabled = false;
                firstFireInOnBeat = true;
                firstFireInOffBeat = false;
            }
            
            
        }
    }

    

    public void RecordMove(Vector2 direction) {
        Debug.Log("Direction " + direction);
        List<Direction> possibleDirections = game.GetPossibleMoveDirections();
        if (Vector2.Dot(new Vector2(1, 1).normalized, direction) > .92 && possibleDirections.Contains(Direction.UpRight)) {
            Debug.Log("Swipe Up Right");
            nextMove = Direction.UpRight;
            
        }
        else if (Vector2.Dot(new Vector2(1, -1).normalized, direction) > .92 && possibleDirections.Contains(Direction.DownRight)) {
            Debug.Log("Swipe Down Right");
            nextMove = Direction.DownRight;
        }
        else if (Vector2.Dot(new Vector2(-1, 1).normalized, direction) > .92 && possibleDirections.Contains(Direction.UpLeft)) {
            Debug.Log("Swipe Up Left");
            nextMove = Direction.UpLeft;
        }
        else if (Vector2.Dot(new Vector2(-1, -1).normalized, direction) > .92 && possibleDirections.Contains(Direction.DownLeft)) {
            Debug.Log("Swipe Down Left");
            nextMove = Direction.DownLeft;
        }
        else if (Vector2.Dot(Vector2.up, direction) > .7 && possibleDirections.Contains(Direction.Up)) {
            Debug.Log("Swipe Up");
            nextMove = Direction.Up;
        }
        else if (Vector2.Dot(Vector2.down, direction) > .7 && possibleDirections.Contains(Direction.Down)) {
            Debug.Log("Swipe Down");
            nextMove = Direction.Down;
        }
        else if (Vector2.Dot(Vector2.left, direction) > .7 && possibleDirections.Contains(Direction.Left)) {
            Debug.Log("Swipe Left");
            nextMove = Direction.Left;
        }
        else if (Vector2.Dot(Vector2.right, direction) > .7 && possibleDirections.Contains(Direction.Right)) {
            Debug.Log("Swipe Right");
            nextMove = Direction.Right;
        }
        board.RemoveMoveIndicators();
        waitingForMove = false;
        swipeDetection.enabled = false;
    }

}
