using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;


public class Board : MonoBehaviour {

    [SerializeField] private int _width, _height;
    [SerializeField] private Field _fieldPrefab;
    [SerializeField] private GameObject _moveIndicatorPrefab;

    [SerializeField] private Chessman _knightPrefab, _bishopPrefab, _pawnPrefab, _rookPrefab, _playerPrefab, _queenPrefab, _kingPrefab;

    private new Camera camera;
    private float halfWidth;
    private float tileWidth;
    private List<Field> fields;

    private List<GameObject> moveIndicators = new List<GameObject>();

    public void Init(int width, int height, List<(int x, int y)> disabledFields, List<(int x, int y)> flagRegion) {
        if(fields != null) {
            foreach(var field in fields) {
                Destroy(field.gameObject);
            }
        }
        _width = width;
        _height = height;
        camera = Camera.main;
        halfWidth = 0.8f * camera.aspect * camera.orthographicSize;
        tileWidth = (halfWidth * 2) / _width;
        fields = new();

        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                if (disabledFields.Contains((x, y))) continue;
                var spawnedField = Instantiate(_fieldPrefab, GetWorldspacePosition(x, y, -tileWidth*0.15f - 0.1f), Quaternion.identity);
                spawnedField.transform.localScale = new Vector3(tileWidth, tileWidth*0.3f, tileWidth);
                spawnedField.name = $"Field {x} {y}";
                spawnedField.transform.parent = transform;

                var isOffset = (x + y) % 2 == 1;
                spawnedField.Init(isOffset, flagRegion.Contains((x, y)));
                fields.Add(spawnedField);
            }
        }
    }

    private Vector3 GetWorldspacePosition(int x, int y, float z = 0) {
        return new Vector3(-halfWidth + x * 2 * halfWidth / _width + tileWidth / 2, z, -(tileWidth * _height / 2) + y * 2 * halfWidth / _width + tileWidth / 2);
    }

    private Vector3 GetWorldspaceScale() {

        return new Vector3(.4f * tileWidth, .4f * tileWidth, .4f * tileWidth);
    }

    public void SetPiece(Chessman piece, int x, int y) {
        piece.transform.position = GetWorldspacePosition(x, y);
        piece.transform.localScale = GetWorldspaceScale();
    }

    public void ShowMoveIndicator(int x, int y) {
        GameObject moveIndicator = Instantiate(_moveIndicatorPrefab, GetWorldspacePosition(x, y), _moveIndicatorPrefab.transform.rotation);
        moveIndicator.transform.parent = transform;
        moveIndicator.transform.localScale = new Vector3(tileWidth * 0.5f, tileWidth * 0.5f);
        moveIndicators.Add(moveIndicator);
    }

    public void RemoveMoveIndicators() {
        moveIndicators.RemoveAll(i => { Destroy(i); return true; }); // destory all move indicators and remove from list
    }

    public Chessman CreatePiece(string name, int x, int y) {
        Chessman prefab;
        switch(name) {
            case "pawn": prefab = _pawnPrefab; break;
            case "knight": prefab = _knightPrefab; break;
            case "bishop": prefab = _bishopPrefab; break;
            case "rook": prefab = _rookPrefab; break;
            case "queen": prefab = _queenPrefab; break;
            case "king": prefab = _kingPrefab; break;
            case "player": prefab = _playerPrefab; break;
            default: Debug.LogError("invalid name for the chessman: " + name); return null;
        }
        Chessman cm = Instantiate(prefab, new Vector3(0, 0, 0), prefab.transform.rotation);
        cm.name = name;
        cm.transform.parent = transform;
        return cm;
    }
}
