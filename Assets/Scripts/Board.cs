using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Board : MonoBehaviour {
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private ChessPiece _chessPiecePrefab;
    [SerializeField] private Tappable _moveIndicatorPrefab;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private Tilemap _tilemap;
    [SerializeField] private RuleTile _tileLight, _tileDark, _tileGoal;
    
    private int _width, _height;
    private float boardScale;
    private readonly List<Tappable> moveIndicators = new();
    private Coroutine runningMoveAnimation;
    
    public Action<Direction> OnMoveIndicatorTapped;

    public void Init(int width, int height, List<(int x, int y)> disabledFields, List<(int x, int y)> flagRegion) {
        _width = width;
        _height = height;
        _tilemap.ClearAllTiles();
        foreach (Transform shadow in _tilemap.transform) {
            Destroy(shadow.gameObject);
        }
    
        var boardWidth = 2 * 0.8f * _mainCamera.aspect * _mainCamera.orthographicSize;
        boardScale = boardWidth / _width;
        
        _tilemap.transform.localScale = new Vector3(boardScale, boardScale, 1);
        _tilemap.transform.position = new Vector3(- _width * boardScale / 2f, -height * boardScale /2f, 0);
        
        for (var x = 0; x < _width; x++) {
            for (var y = 0; y < _height; y++) {
                if (disabledFields.Contains((x, y))) continue;
                if (flagRegion.Contains((x, y))) _tilemap.SetTile(new Vector3Int(x, y, 0), _tileGoal);
                else _tilemap.SetTile(new Vector3Int(x, y, 0), (x + y) % 2 == 0 ? _tileLight : _tileDark);
            }
        }
        
        for (var x = 0; x < _width; x++) {
            for (var y = 0; y < _height; y++) {
                var currentTile = _tilemap.GetTile<ChessboardRuleTile>(new Vector3Int(x, y, 0));
                if (currentTile != null) {
                    currentTile.AddShadow(new Vector3Int(x, y, 0), _tilemap);
                }
            }
        }
    }

    private Vector3 GetWorldSpacePosition(int x, int y, int z = 0) {

        var worldSpaceX = (x + 0.5f) * boardScale;
        var worldSpaceY = (y + 0.5f) * boardScale * 0.75f;

        return _tilemap.transform.position + new Vector3(worldSpaceX, worldSpaceY, z);
    }

    private Vector3 GetWorldSpaceScale(int width, int height) {

        return new Vector3(.4f * boardScale * width, .4f * boardScale * height);
    }

    public void SetPiece(ChessPiece piece, int x, int y) {
        piece.transform.position = GetWorldSpacePosition(x, y, y);
        piece.transform.localScale = GetWorldSpaceScale(1, 1);
    }

    public void MovePiece(ChessPiece piece, int x, int y) {
        runningMoveAnimation = StartCoroutine(AnimatedMove(piece, x, y));
    }
    
    public void CancelMoveAnimation() {
        if (runningMoveAnimation == null) return;
        StopCoroutine(runningMoveAnimation);
        runningMoveAnimation = null;
    }

    private IEnumerator AnimatedMove(ChessPiece piece, int x, int y) {
        var destination = GetWorldSpacePosition(x, y, y);
        while (piece.transform.position != destination) {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, destination, _movementSpeed * boardScale * Time.deltaTime);
            yield return null;
        }
    }

    private static IEnumerator BumpAnimation(Transform target, Vector2 direction, float distance = 0.1f, float duration = 0.2f) {
        var startPos = target.localPosition;
        var bumpPos = startPos + (Vector3)(direction.normalized * distance);
        
        var t = 0f;
        while (t < 1f) {
            if(!target) yield break; // check if target is destroyed
            t += Time.deltaTime / (duration / 2f);
            target.localPosition = Vector3.Lerp(startPos, bumpPos, t);
            yield return null;
        }

        t = 0f;
        while (t < 1f) {
            if(!target) yield break; // check if target is destroyed
            t += Time.deltaTime / (duration / 2f);
            target.localPosition = Vector3.Lerp(bumpPos, startPos, t);
            yield return null;
        }
        
    }
    
    public void ShowMoveIndicator(int x, int y, Direction direction) {
        var moveIndicator = Instantiate(_moveIndicatorPrefab, GetWorldSpacePosition(x, y, -1), Quaternion.identity);
        moveIndicator.transform.parent = transform;
        moveIndicator.transform.localScale = new Vector3(boardScale * 0.5f, boardScale * 0.5f);
        moveIndicator.OnTapped += () => {
            OnMoveIndicatorTapped.Invoke(direction);
        };
        
        var spriteRenderers = moveIndicator.GetComponentsInChildren<SpriteRenderer>();
        var angle = direction switch {
            Direction.Right => 0f,
            Direction.Up => 90f,
            Direction.Left => 180f,
            Direction.Down => 270f,
            _ => 0f
        };
        foreach (var spriteRenderer in spriteRenderers) {
            spriteRenderer.transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        var bumpDirection = direction switch {
            Direction.Right => Vector2.right,
            Direction.Up => Vector2.up,
            Direction.Left => Vector2.left,
            Direction.Down => Vector2.down,
            _ => Vector2.zero
        };
        StartCoroutine(BumpAnimation(moveIndicator.transform, bumpDirection));
        
        moveIndicators.Add(moveIndicator);
    }

    public void RemoveMoveIndicators() {
        moveIndicators.RemoveAll(i => { Destroy(i.gameObject); return true; }); // destroy all move indicators and remove from list
    }

    public ChessPiece CreatePiece(string name, int x, int y) {
        var chessPiece = Instantiate(_chessPiecePrefab, new Vector3(0, 0, -1), Quaternion.identity);
        chessPiece.name = name;
        chessPiece.transform.parent = transform;
        chessPiece.Init();
        return chessPiece;
    }
}
