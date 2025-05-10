using System;
using UnityEngine;
using System.Collections.Generic;

public enum PieceType {
    PAWN,
    KNIGHT,
    ROOK,
    BISHOP,
    QUEEN,
    PLAYER,
};

public class ChessPiece : MonoBehaviour {
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameObject _shadow;
    [SerializeField] private Sprite _player, _pawn, _knight, _bishop, _rook, _queen;

    private int x = -1;
    private int y = -1;
    private PieceType piecetype;
    
    private Dictionary<PieceType, float> shadowScale = new() {
        { PieceType.QUEEN, 1.0f },
        { PieceType.BISHOP, 1.95f },
        { PieceType.KNIGHT, 2f },
        { PieceType.ROOK, 1.8f },
        { PieceType.PAWN, 1.7f },
        { PieceType.PLAYER, 1.7f }
    };
    
    public int GetX() { return x; }
    public void SetX(int x) { this.x = x; }

    public int GetY() { return y; }
    public void SetY(int y) { this.y = y; }

    public void Init() {
        piecetype = name switch {
            "pawn" => PieceType.PAWN,
            "knight" => PieceType.KNIGHT,
            "bishop" => PieceType.BISHOP,
            "rook" => PieceType.ROOK,
            "queen" => PieceType.QUEEN,
            "player" => PieceType.PLAYER,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _spriteRenderer.sprite = name switch {
            "pawn" => _pawn,
            "knight" => _knight,
            "bishop" => _bishop,
            "rook" => _rook,
            "queen" => _queen,
            "player" => _player,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _shadow.transform.localScale = new Vector3(shadowScale[piecetype], shadowScale[piecetype], 1);
    }
}

