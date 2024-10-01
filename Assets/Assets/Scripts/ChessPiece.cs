using System;
using UnityEngine;
using System.Collections.Generic;

public enum PieceType {
    PAWN,
    KNIGHT,
    ROOK,
    BISHOP,
    QUEEN,
    KING,
    PLAYER,
};

public class ChessPiece : MonoBehaviour {
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private Sprite _player, _pawn, _knight, _bishop, _rook, _queen, _king;

    private int x = -1;
    private int y = -1;
    private GameObject player;
    private PieceType piecetype;
    private Transform eyeLeft;
    private Transform eyeRight;
    
    private Dictionary<PieceType, Vector2> leftEyePositions = new() {
        { PieceType.KING, new Vector2(3.5f, 4.2f) },
        { PieceType.QUEEN, new Vector2(5.1f, 2.3f) },
        { PieceType.BISHOP, new Vector2(2.4f, 6.7f) },
        { PieceType.KNIGHT, new Vector2(-0.613f, 2.158f) },
        { PieceType.ROOK, new Vector2(4.0f, 2.0f) },
        { PieceType.PAWN, new Vector2(0.5f, 1.1f) }
    };

    private Dictionary<PieceType, Vector2> rightEyePositions = new() {
        { PieceType.KING, new Vector2(3.5f, 4.2f) },
        { PieceType.QUEEN, new Vector2(5.1f, 2.3f) },
        { PieceType.BISHOP, new Vector2(2.4f, 6.7f) },
        { PieceType.KNIGHT, new Vector2(-0.039f, 1.972f) },
        { PieceType.ROOK, new Vector2(4.0f, 2.0f) },
        { PieceType.PAWN, new Vector2(0.5f, 1.1f) }
    };
    
    public int GetX() { return x; }
    public void SetX(int x) { this.x = x; }

    public int GetY() { return y; }
    public void SetY(int y) { this.y = y; }

    public void Init() {
        player = GameObject.Find("player");

        piecetype = name switch {
            "pawn" => PieceType.PAWN,
            "knight" => PieceType.KNIGHT,
            "bishop" => PieceType.BISHOP,
            "rook" => PieceType.ROOK,
            "queen" => PieceType.QUEEN,
            "king" => PieceType.KING,
            "player" => PieceType.PLAYER,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        _spriteRenderer.sprite = name switch {
            "pawn" => _pawn,
            "knight" => _knight,
            "bishop" => _bishop,
            "rook" => _rook,
            "queen" => _queen,
            "king" => _king,
            "player" => _player,
            _ => throw new ArgumentOutOfRangeException()
        };

        eyeLeft = transform.Find("EyeLeft");
        eyeRight = transform.Find("EyeRight");
        
        if (piecetype == PieceType.PLAYER) {
            eyeLeft.gameObject.SetActive(false);
            eyeRight.gameObject.SetActive(false);
        } else {
            eyeLeft.localPosition = leftEyePositions[piecetype];
            eyeRight.localPosition = rightEyePositions[piecetype];
        } 
    }

    public void Update() {
        if (piecetype != PieceType.PLAYER) {
            var eyeDirection = (player.transform.position - (transform.position + new Vector3(0, 0.5f, 0))).normalized;
            eyeLeft.localPosition = ((Vector3) leftEyePositions[piecetype]) + eyeDirection * .1f;
            eyeRight.localPosition = ((Vector3) rightEyePositions[piecetype]) + eyeDirection * .1f;
        }
        
    }
}

