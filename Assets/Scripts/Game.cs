using System;
using System.Collections.Generic;
using UnityEngine;

public class Game : MonoBehaviour {

    [SerializeField] private Board _board;
    
    private int width, height;
    private List<(int x, int y)> disabledFields, flagRegion;
    private List<(string pieceName, int x, int y)> initPosition;
    private bool piecesCreated;
    private bool ended;
    private bool won;
    private ChessPiece[,] position;
    private ChessPiece player;

    private List<ChessPiece> pieces;
    

    public void Init(List<(string pieceName, int x, int y)> initPosition, int width, int height, List<(int x, int y)> disabledFields, List<(int x, int y)> flagRegion) {
        if(pieces != null) {
            foreach(var piece in pieces) {
                DestroyImmediate(piece.gameObject);
            }
        }
        this.width = width;
        this.height = height;
        this.disabledFields = disabledFields;
        this.flagRegion = flagRegion;
        this.initPosition = initPosition;
        piecesCreated = false;

        pieces = new List<ChessPiece>();
        _board.Init(width, height, disabledFields, flagRegion);
        SetupLevel();
    }

    public void SetupLevel() {
        ended = false;
        _board.CancelMoveAnimation();
        position = new ChessPiece[width, height];
        
        // put player piece to start
        for (int i = 0; i < initPosition.Count; i++) {
            if (initPosition[i].pieceName == "player") {
                (initPosition[0], initPosition[i]) = (initPosition[i], initPosition[0]);
                break;
            }
        }
        
        for (var i = 0; i < initPosition.Count; i++) {
            var (pieceName, x, y) = initPosition[i];
            // Init Pieces
            if (!piecesCreated) {
                var piece = _board.CreatePiece(pieceName, x, y);
                MovePiece(piece, x, y);
                pieces.Add(piece);
                if (pieceName == "player") player = piece;
            } else {
                // if pieces are already created just move them
                pieces[i].SetX(-1); // set x and y to -1 so that it doesn't cause conflicts with other pieces
                pieces[i].SetY(-1);
                MovePiece(pieces[i], x, y);
            }
        }    
        piecesCreated = true;
    }

    public List<Direction> GetPossibleMoveDirections() {
        var x = player.GetX();
        var y = player.GetY();
        List<Direction> possibleMoves = new();
        // straight moves if no piece is on position
        if (IsPositionOnBoard(x - 1, y) && !disabledFields.Contains((x - 1, y))) possibleMoves.Add(Direction.Left);
        if (IsPositionOnBoard(x, y - 1) && !disabledFields.Contains((x, y - 1))) possibleMoves.Add(Direction.Down);
        if (IsPositionOnBoard(x + 1, y) && !disabledFields.Contains((x + 1, y)))  possibleMoves.Add(Direction.Right);
        if (IsPositionOnBoard(x, y + 1) && !disabledFields.Contains((x, y + 1))) possibleMoves.Add(Direction.Up);
        // diagonal moves if a piece is on position
        if (IsPositionOnBoard(x - 1, y - 1) && position[x - 1, y - 1] is not null) possibleMoves.Add(Direction.DownLeft);
        if (IsPositionOnBoard(x - 1, y + 1) && position[x - 1, y + 1] is not null) possibleMoves.Add(Direction.UpLeft);
        if (IsPositionOnBoard(x + 1, y - 1) && position[x + 1, y - 1] is not null) possibleMoves.Add(Direction.DownRight);
        if (IsPositionOnBoard(x + 1, y + 1) && position[x + 1, y + 1] is not null) possibleMoves.Add(Direction.UpRight);
        return possibleMoves;
    }

    public void ShowPossibleMoves() {
        var possibleMoves = GetPossibleMoves();
        player.OutlineEnabled = true;
        foreach (var (move, direction) in possibleMoves) {
            if (position[(int)move.x, (int)move.y] is null) {
                _board.ShowMoveIndicator((int)move.x, (int)move.y, direction);
            } else {
                position[(int)move.x, (int)move.y].OutlineEnabled = true;
            }
        }
    }
    
    public void RemoveMoveIndicators() {
        _board.RemoveMoveIndicators();
        player.OutlineEnabled = false;
        foreach (var piece in pieces) {
            piece.OutlineEnabled = false;
        }
    }

    // moves the player in the given direction and return true if move finished game
    public bool Move(Direction direction) {
        return direction switch {
            Direction.Up => MovePiece(player, player.GetX(), player.GetY() + 1),
            Direction.Down => MovePiece(player, player.GetX(), player.GetY() - 1),
            Direction.Left => MovePiece(player, player.GetX() - 1, player.GetY()),
            Direction.Right => MovePiece(player, player.GetX() + 1, player.GetY()),
            Direction.UpRight => MovePiece(player, player.GetX() + 1, player.GetY() + 1),
            Direction.DownRight => MovePiece(player, player.GetX() + 1, player.GetY() - 1),
            Direction.UpLeft => MovePiece(player, player.GetX() - 1, player.GetY() + 1),
            Direction.DownLeft => MovePiece(player, player.GetX() - 1, player.GetY() - 1),
            Direction.None => false,
            _ => false
        };
    }

    // moves enemy piece from.(x,y) to.(x,y) and returns true if move ended the game
    public bool MoveEnemy((int x, int y) from, (int x, int y) to) {
        return MovePiece(position[from.x, from.y], to.x, to.y);
    }

    public ChessPiece[,] GetPosition() {
        return position;
    }

    public bool GetWinningStatus() {
        return won;
    }

    // moves piece cm to (x,y) and returns true if the move ended the game
    private bool MovePiece(ChessPiece cm, int x, int y) {
        (int x, int y) from = (cm.GetX(), cm.GetY());
        // check for win
        if (flagRegion.Contains((x, y)) && cm.name == "player") {
            ended = true;
            won = true;
        }
        if (position[x, y] is not null) {
            if(position[x, y].name == "player") { // check for loose
                ended = true;
                won = false;
            }
            position[x, y].transform.localScale = new Vector3(0,0,0);
            position[x, y].SetX(-1);
            position[x, y].SetY(-1);
        }
        
        cm.SetX(x);
        cm.SetY(y);
        if (IsPositionOnBoard(from.x, from.y)) {
            SetPositionEmpty(from.x, from.y);
            _board.MovePiece(cm, x, y);
        } else {
            _board.SetPiece(cm, x, y);
        }
        position[x, y] = cm;

        return ended;
    }

    private void SetPositionEmpty(int x, int y) {
        position[x, y] = null;
    }

    private bool IsPositionOnBoard(int x, int y) {
        return x >= 0 && y >= 0 && x < width && y < height;
    }

    private List<(Vector2, Direction)> GetPossibleMoves() {
        var possibleMoveDirections = GetPossibleMoveDirections();

        var x = player.GetX();
        var y = player.GetY();
        var possibleMoves = new List<(Vector2, Direction)>();
        foreach (var moveDirection in possibleMoveDirections) {
            switch(moveDirection) {
                case Direction.Left: possibleMoves.Add((new Vector2(x - 1, y), moveDirection)); break;
                case Direction.Right: possibleMoves.Add((new Vector2(x + 1, y), moveDirection)); break;
                case Direction.Up: possibleMoves.Add((new Vector2(x, y +1 ), moveDirection)); break;
                case Direction.Down: possibleMoves.Add((new Vector2(x, y - 1), moveDirection)); break;
                case Direction.UpLeft: possibleMoves.Add((new Vector2(x - 1, y + 1), moveDirection)); break;
                case Direction.UpRight: possibleMoves.Add((new Vector2(x + 1, y + 1), moveDirection)); break;
                case Direction.DownLeft: possibleMoves.Add((new Vector2(x - 1, y - 1), moveDirection)); break;
                case Direction.DownRight: possibleMoves.Add((new Vector2(x + 1, y - 1), moveDirection)); break;
                case Direction.None:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        return possibleMoves;
    }
}

public enum Direction {
    Up, Down, Left, Right, UpRight, DownRight, UpLeft, DownLeft, None
}
