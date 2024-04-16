using UnityEngine;

public class ChessPiece : MonoBehaviour {
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private int x = -1;
    private int y = -1;

    [SerializeField] private Sprite _player, _pawn, _knight, _bishop, _rook, _queen, _king;

    public int GetX() { return x; }
    public void SetX(int x) { this.x = x; }

    public int GetY() { return y; }
    public void SetY(int y) { this.y = y; }

    public void Init() {
        _spriteRenderer.sprite = name switch {
            "pawn" => _pawn,
            "knight" => _knight,
            "bishop" => _bishop,
            "rook" => _rook,
            "queen" => _queen,
            "king" => _king,
            "player" => _player,
            _ => _spriteRenderer.sprite
        };
    }
}

