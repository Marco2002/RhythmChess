using UnityEngine;

public class Controller : MonoBehaviour {
    // Start is called before the first frame update
    [SerializeField] private Game _game;
    [SerializeField] private Board _board;
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private SwipeDetection _swipeDetection;
    [SerializeField] private LevelReader _levelReader;
    [SerializeField] private Background _background;
    [SerializeField] private BeatManager _beatManager;
    
    [SerializeField] private int _level = 1;
    
    private bool gameEnded;
    private Direction nextMove = Direction.None;

    private void Start() {
        Debug.Log("started loading level");
        Application.targetFrameRate = 60;
        _levelReader.ReadLevelCsv("level"+_level);
        PrepareGame();
        _game.Init(_levelReader.GetStartingPosition(), _levelReader.GetMaxFile(), _levelReader.GetMaxRank(), _levelReader.GetDisabledFields(), _levelReader.GetFlagRegion());
        Debug.Log("game initialized");
    }

    public void HandleOnBeat() {
        if (gameEnded) {
            var playerWon = _game.GetWinningStatus();
            if (playerWon) {
                LoadNextLevel();
            }
            else {
                ResetLevel();
            }
        } else {
            // StartCoroutine(PlaySound(_audioRide));
            _game.ShowPossibleMoves();
            _swipeDetection.enabled = true;
        }
    }

    public void HandeOffBeat() {
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
        _beatManager.Reset();
        SetColoring(Coloring.Primary);
    }

    private void ResetLevel() {
        _game.SetupLevel();
        PrepareGame();
    }

    private void LoadNextLevel() {
        _level++;
        _levelReader.ReadLevelCsv("level" + _level);
        PrepareGame();
        _game.Init(_levelReader.GetStartingPosition(), _levelReader.GetMaxFile(), _levelReader.GetMaxRank(), _levelReader.GetDisabledFields(), _levelReader.GetFlagRegion());
    }

    private void SetColoring(Coloring coloring) {
        _background.SetColoring(coloring);
        _board.SetColoring(coloring);
    }

    public void SetColoringPrimary() {
        _background.SetColoring(Coloring.Primary);
        _board.SetColoring(Coloring.Primary);
    }
    
    public void SetColoringSecondary() {
        _background.SetColoring(Coloring.Secondary);
        _board.SetColoring(Coloring.Secondary);
    }
}
