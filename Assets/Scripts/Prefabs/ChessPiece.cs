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
    private SpriteRenderer[] outlines;
    
    private Dictionary<PieceType, float> shadowScale = new() {
        { PieceType.QUEEN, 1.0f },
        { PieceType.BISHOP, 1.95f },
        { PieceType.KNIGHT, 2f },
        { PieceType.ROOK, 1.8f },
        { PieceType.PAWN, 1.7f },
        { PieceType.PLAYER, 1.7f }
    };
    
    public bool OutlineEnabled {
        get => outlines != null && outlines[0].enabled;
        set {
            if (outlines == null) return;
            foreach (var outlineSpriteRenderer in outlines) {
                outlineSpriteRenderer.enabled = value;
            }
        }
    }
    public int GetX() { return x; }
    public void SetX(int x) { this.x = x; }

    public int GetY() { return y; }

    public void SetY(int y) {
        this.y = y;
        _spriteRenderer.sortingOrder = -2*y;
        foreach (var outlineSpriteRenderer in outlines) {
            outlineSpriteRenderer.sortingOrder = -2*y - 1;
        }
    }
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
        
        var sprite = name switch {
            "pawn" => _pawn,
            "knight" => _knight,
            "bishop" => _bishop,
            "rook" => _rook,
            "queen" => _queen,
            "player" => _player,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        var outline = transform.Find("Outline");
        if (outline != null) {
            outlines = outline.GetComponentsInChildren<SpriteRenderer>();
            foreach (var outlineSpriteRenderer in outlines) {
                outlineSpriteRenderer.sprite = sprite;
                outlineSpriteRenderer.enabled = false;
            }
        }
        _spriteRenderer.sprite = sprite;
        var _collider = GetComponent<CapsuleCollider2D>();
        _collider.enabled = name != "player";
        
        _shadow.transform.localScale = new Vector3(shadowScale[piecetype], shadowScale[piecetype], 1);
    }
}

