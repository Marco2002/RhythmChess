using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Game : MonoBehaviour {

    private int width, height;
    private List<(int x, int y)> disabledFields, flagRegion;
    List<(string pieceName, int x, int y)> initPosition;
    bool piecesCreated = false;
    bool ended = false;
    bool won = false;
    private Chessman[,] position;
    private Chessman player;

    private List<Chessman> pieces;
    [SerializeField] private Board board;

    public void Init(List<(string pieceName, int x, int y)> initPosition, int width, int height, List<(int x, int y)> disabledFields, List<(int x, int y)> flagRegion) {
        if(pieces != null) {
            foreach(var piece in pieces) {
                Destroy(piece.gameObject);
            }
        }
        this.width = width;
        this.height = height;
        this.disabledFields = disabledFields;
        this.flagRegion = flagRegion;
        this.initPosition = initPosition;
        this.piecesCreated = false;

        pieces = new();
        board.Init(width, height, disabledFields, flagRegion);
        SetupLevel();
    }

    public void SetupLevel() {
        ended = false;

        position = new Chessman[width, height];
        // Init pieces
        for (int i = 0; i < initPosition.Count(); i++) {
            var (pieceName, x, y) = initPosition[i];
            // Init Pieces
            if (!piecesCreated) {
                var piece = board.CreatePiece(pieceName, (int)x, (int)y);
                MovePiece(piece, x, y);
                pieces.Add(piece);
                if (pieceName == "player") {
                    player = piece;
                }
            } else {
                // if pieces are already created just move them
                MovePiece(pieces[i], x, y);
            }
           

        }    
        piecesCreated = true;
    }

    public List<Direction> GetPossibleMoveDirections() {
        int x = player.GetX();
        int y = player.GetY();
        List<Direction> possibleMoves = new();
        // straight moves if no piece is on position
        if (IsPositionOnBoard(x - 1, y) && !disabledFields.Contains((x - 1, y))) possibleMoves.Add(Direction.Left);
        if (IsPositionOnBoard(x, y - 1) && !disabledFields.Contains((x, y - 1))) possibleMoves.Add(Direction.Down);
        if (IsPositionOnBoard(x + 1, y) && !disabledFields.Contains((x + 1, y)))  possibleMoves.Add(Direction.Right);
        if (IsPositionOnBoard(x, y + 1) && !disabledFields.Contains((x, y + 1))) possibleMoves.Add(Direction.Up);
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

    // moves the player in the given direction and return true if move finished game
    public bool Move(Direction direction) {
        switch (direction) {
            case Direction.Up: return MovePiece(player, player.GetX(), player.GetY() + 1);
            case Direction.Down: return MovePiece(player, player.GetX(), player.GetY() - 1);
            case Direction.Left: return MovePiece(player, player.GetX() - 1, player.GetY());
            case Direction.Right: return MovePiece(player, player.GetX() + 1, player.GetY());
            case Direction.UpRight: return MovePiece(player, player.GetX() + 1, player.GetY() + 1);
            case Direction.DownRight: return MovePiece(player, player.GetX() + 1, player.GetY() - 1);
            case Direction.UpLeft: return MovePiece(player, player.GetX() - 1, player.GetY() + 1);
            case Direction.DownLeft: return MovePiece(player, player.GetX() - 1, player.GetY() - 1);
            case Direction.None:
                return false ;
        }
        Debug.LogError("invalid move direction for the method Move in the Game component");
        return false;
    }

    // moves enemy piece from from.(x,y) to to.(x,y) and returns true if move ended the game
    public bool MoveEnemy((int x, int y) from, (int x, int y) to) {
        return MovePiece(position[from.x, from.y], to.x, to.y);
    }

    public Chessman[,] GetPosition() {
        return position;
    }

    public bool GetWinningSatus() {
        if (!ended) Debug.LogWarning("called GetWinningStatus before game ended");
        return won;
    }

    


    // moves piece cm to (x,y) and returns true if the move ended the game
    private bool MovePiece(Chessman cm, int x, int y) {
        (int x, int y) from = (cm.GetX(), cm.GetY());
        // check for win
        if (flagRegion.Contains((x, y)) && cm.name == "player") {
            ended = true;
            won = true;
        }
        if (position[x, y] != null) {
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
            board.MovePiece(cm, x, y);
        } else {
            board.SetPiece(cm, x, y);
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

    private List<Vector2> GetPossibleMoves() {
        List<Direction> possibleMoveDirections = GetPossibleMoveDirections();

        int x = player.GetX();
        int y = player.GetY();
        List<Vector2> possibleMoves = new List<Vector2>();
        foreach (Direction moveDirection in possibleMoveDirections) {
            switch(moveDirection) {
                case Direction.Left: possibleMoves.Add(new Vector2(x - 1, y)); break;
                case Direction.Right: possibleMoves.Add(new Vector2(x + 1, y)); break;
                case Direction.Up: possibleMoves.Add(new Vector2(x, y +1 )); break;
                case Direction.Down: possibleMoves.Add(new Vector2(x, y - 1)); break;
                case Direction.UpLeft: possibleMoves.Add(new Vector2(x - 1, y + 1)); break;
                case Direction.UpRight: possibleMoves.Add(new Vector2(x + 1, y + 1)); break;
                case Direction.DownLeft: possibleMoves.Add(new Vector2(x - 1, y - 1)); break;
                case Direction.DownRight: possibleMoves.Add(new Vector2(x + 1, y - 1)); break;
            }
        }
        return possibleMoves;
    }
}

public enum Direction {
    Up, Down, Left, Right, UpRight, DownRight, UpLeft, DownLeft, None
}
