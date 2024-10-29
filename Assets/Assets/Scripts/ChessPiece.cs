using System;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Serialization;

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
    [SerializeField] private Sprite _player, _pawn, _knight, _bishop, _rook, _queen, _eye, _eyeRook;
    [SerializeField] private SpriteRenderer _eyeLeft;
    [SerializeField] private SpriteRenderer _eyeRight;

    private int x = -1;
    private int y = -1;
    private GameObject player;
    private PieceType piecetype;
    
    private Dictionary<PieceType, float> eyeScale = new() {
        { PieceType.QUEEN, 1.0f },
        { PieceType.BISHOP, 1.2f },
        { PieceType.KNIGHT, 1.0f },
        { PieceType.ROOK, 1.3f },
        { PieceType.PAWN, 1.2f },
        { PieceType.PLAYER, 1.0f }
    };
    
    private Dictionary<PieceType, Vector2> leftEyePositions = new() {
        { PieceType.QUEEN, new Vector2(5.1f, 2.3f) },
        { PieceType.BISHOP, new Vector2(-0.37f, 1.967f) },
        { PieceType.KNIGHT, new Vector2(-0.613f, 2.158f) },
        { PieceType.ROOK, new Vector2(-0.45f, 1.797f) },
        { PieceType.PAWN, new Vector2(-0.485f, 1.557f) }
    };

    private Dictionary<PieceType, Vector2> rightEyePositions = new() {
        { PieceType.QUEEN, new Vector2(5.1f, 2.3f) },
        { PieceType.BISHOP, new Vector2(0.37f, 1.967f) },
        { PieceType.KNIGHT, new Vector2(-0.039f, 1.972f) },
        { PieceType.ROOK, new Vector2(0.45f, 1.797f) },
        { PieceType.PAWN, new Vector2(0.485f, 1.575f) }
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

        _eyeLeft.transform.localScale = new(eyeScale[piecetype], eyeScale[piecetype], 1f);
        _eyeRight.transform.localScale = new(eyeScale[piecetype], eyeScale[piecetype], 1f);

        _eyeRight.sprite = piecetype == PieceType.ROOK ? _eyeRook : _eye;
        _eyeLeft.sprite = piecetype == PieceType.ROOK ? _eyeRook : _eye;
        
        if (piecetype == PieceType.PLAYER) {
            _eyeLeft.gameObject.SetActive(false);
            _eyeRight.gameObject.SetActive(false);
        } else {
            _eyeLeft.transform.localPosition = leftEyePositions[piecetype];
            _eyeRight.transform.localPosition = rightEyePositions[piecetype];
        } 
    }

    public void Update() {
        if (piecetype != PieceType.PLAYER) {
            var eyeDirection = (player.transform.position - (transform.position + new Vector3(0, 0.5f, 0))).normalized;
            _eyeLeft.transform.localPosition =
                ((Vector3)leftEyePositions[piecetype]) + eyeDirection * (.1f * eyeScale[piecetype]);
            _eyeRight.transform.localPosition =
                ((Vector3) rightEyePositions[piecetype]) + eyeDirection * (.1f * eyeScale[piecetype]);
        }
    }
}

