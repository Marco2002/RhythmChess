using System.Collections;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEditor.Search;
using UnityEngine;

public class Game : MonoBehaviour {
    [SerializeField] private Chessman chesspiece;
    [SerializeField] private Chessman playerPrefab;

    private int width, height;
    private List<(int x, int y)> disabledFields, flagRegion;
    private Chessman[,] position;
    private Player player;

    private List<Chessman> pieces = new List<Chessman>();
    [SerializeField] private Board board;

    public void Init(List<(string pieceName, int x, int y)> initPosition, int width, int height, List<(int x, int y)> disabledFields, List<(int x, int y)> flagRegion) {
        this.width = width;
        this.height = height;
        this.disabledFields = disabledFields;
        this.flagRegion = flagRegion;

        board.Init(width, height, disabledFields, flagRegion);
        position = new Chessman[width, height];
        // Init pieces
        foreach(var piece in initPosition) {
            if(piece.pieceName == "player") {
                // Init Player
                player = Instantiate(playerPrefab, new Vector3(0, 0, -1), Quaternion.identity).GetComponent<Player>();
                player.name = "player";
                MovePiece(player, (int)piece.x, (int)piece.y);
                player.Activate();
            } else {
                pieces.Add(CreatePiece(piece.pieceName, (int)piece.x, (int)piece.y));
            }
            
        }
    }

    public List<Direction> GetPossibleMoveDirections() {
        int x = player.GetX();
        int y = player.GetY();
        List<Direction> possibleMoves = new List<Direction>();
        // straight moves if no piece is on position
        if (IsPositionOnBoard(x - 1, y) && position[x - 1, y] == null && !disabledFields.Contains((x - 1, y))) possibleMoves.Add(Direction.Left);
        if (IsPositionOnBoard(x, y - 1) && position[x, y - 1] == null && !disabledFields.Contains((x, y - 1))) possibleMoves.Add(Direction.Down);
        if (IsPositionOnBoard(x + 1, y) && position[x + 1, y] == null && !disabledFields.Contains((x + 1, y)))  possibleMoves.Add(Direction.Right);
        if (IsPositionOnBoard(x, y + 1) && position[x, y + 1] == null && !disabledFields.Contains((x, y + 1))) possibleMoves.Add(Direction.Up);
        // diagonal moves if a piece is on position
        if (IsPositionOnBoard(x - 1, y - 1) && position[x - 1, y - 1] != null) possibleMoves.Add(Direction.DownLeft);
        if (IsPositionOnBoard(x - 1, y + 1) && position[x - 1, y + 1] != null) possibleMoves.Add(Direction.UpLeft);
        if (IsPositionOnBoard(x + 1, y - 1) && position[x + 1, y - 1] != null) possibleMoves.Add(Direction.DownRight);
        if (IsPositionOnBoard(x + 1, y + 1) && position[x + 1, y + 1] != null) possibleMoves.Add(Direction.UpRight);
        return possibleMoves;
    }

    public void ShowPossibleMoves() {
        List<Vector2> possibleMoves = GetPossibleMoves();
        foreach (var move in possibleMoves) {
            board.ShowMoveIndicator((int)move.x, (int)move.y);
        }

    }

    public void Move(Direction direction) {
        switch (direction) {
            case Direction.Up: MovePiece(player, player.GetX(), player.GetY() + 1); break;
            case Direction.Down: MovePiece(player, player.GetX(), player.GetY() - 1); break;
            case Direction.Left: MovePiece(player, player.GetX() - 1, player.GetY()); break;
            case Direction.Right: MovePiece(player, player.GetX() + 1, player.GetY()); break;
            case Direction.UpRight: MovePiece(player, player.GetX() + 1, player.GetY() + 1); break;
            case Direction.DownRight: MovePiece(player, player.GetX() + 1, player.GetY() - 1); break;
            case Direction.UpLeft: MovePiece(player, player.GetX() - 1, player.GetY() + 1); break;
            case Direction.DownLeft: MovePiece(player, player.GetX() - 1, player.GetY() - 1); break;
        }
    }
    public void MoveEnemy((int x, int y) from, (int x, int y) to) {
        MovePiece(position[from.x, from.y], to.x, to.y);
    }

    public Chessman[,] GetPosition() {
        return position;
    }

    private Chessman CreatePiece(string name, int x, int y) {
        Chessman cm = Instantiate(chesspiece, new Vector3(0, 0, -1), Quaternion.identity);
        cm.name = name;
        MovePiece(cm, x, y);
        cm.Activate();
        return cm;
    }


    private void MovePiece(Chessman cm, int x, int y) {
        if (IsPositionOnBoard(cm.GetX(), cm.GetY())) SetPositionEmpty(cm.GetX(), cm.GetY());
        position[x, y] = cm;
        cm.SetX(x);
        cm.SetY(y);
        board.SetPiece(cm, cm.GetX(), cm.GetY());
    }

    private void SetPositionEmpty(int x, int y) {
        position[x, y] = null;
    }

    private bool IsPositionOnBoard(int x, int y) {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    private List<Vector2> GetPossibleMoves() {
        int x = player.GetX();
        int y = player.GetY();
        List<Vector2> possibleMoves = new List<Vector2>();
        // straight moves if no piece is on position
        if (IsPositionOnBoard(x - 1, y) && position[x - 1, y] == null && !disabledFields.Contains((x - 1, y))) possibleMoves.Add(new Vector2(x - 1, y));
        if (IsPositionOnBoard(x, y - 1) && position[x, y - 1] == null && !disabledFields.Contains((x, y - 1))) possibleMoves.Add(new Vector2(x, y - 1));
        if (IsPositionOnBoard(x + 1, y) && position[x + 1, y] == null && !disabledFields.Contains((x + 1, y))) possibleMoves.Add(new Vector2(x + 1, y));
        if (IsPositionOnBoard(x, y + 1) && position[x, y + 1] == null && !disabledFields.Contains((x, y + 1))) possibleMoves.Add(new Vector2(x, y + 1));
        // diagonal moves if a piece is on position
        if (IsPositionOnBoard(x - 1, y - 1) && position[x - 1, y - 1] != null) possibleMoves.Add(new Vector2(x - 1, y - 1));
        if (IsPositionOnBoard(x - 1, y + 1) && position[x - 1, y + 1] != null) possibleMoves.Add(new Vector2(x - 1, y + 1));
        if (IsPositionOnBoard(x + 1, y - 1) && position[x + 1, y - 1] != null) possibleMoves.Add(new Vector2(x + 1, y - 1));
        if (IsPositionOnBoard(x + 1, y + 1) && position[x + 1, y + 1] != null) possibleMoves.Add(new Vector2(x + 1, y + 1));
        return possibleMoves;
    }
}

public enum Direction {
    Up, Down, Left, Right, UpRight, DownRight, UpLeft, DownLeft, None
}
