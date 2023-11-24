using System.Collections.Generic;
using UnityEngine;


public class Board : MonoBehaviour {
    [SerializeField] private int _width, _height;
    [SerializeField] private Field _fieldPrefab;
    [SerializeField] private Chessman _chesspiecePrefab;
    [SerializeField] private GameObject _moveIndicatorPrefab;

    private new Camera camera;
    private (float zeroReferenceX, float zeroReferenceY ) boardZeroReference;
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
        float boardWidth = 2 * 0.8f * camera.aspect * camera.orthographicSize;
        tileWidth = boardWidth / _width;
        float uiHeaderHeight = 0.1f * camera.orthographicSize;
        if(_height * tileWidth > camera.orthographicSize*0.8*2) {
            // board is too tall -> tileWidth needs to be set based on the screen height and not screen width;
            tileWidth = 2 * 0.8f * camera.orthographicSize / _height;
        }
        boardZeroReference = (-(tileWidth * _width / 2) + tileWidth / 2, -(tileWidth * _height / 2) + tileWidth / 2 - uiHeaderHeight); // the coordinates of the (0, 0) file in unity units
        fields = new();

        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                var spawnedField = Instantiate(_fieldPrefab, GetWorldspacePosition(x, y, 1), Quaternion.identity);
                spawnedField.transform.localScale = new Vector3(tileWidth, tileWidth);
                spawnedField.name = $"Field {x} {y}";
                spawnedField.transform.parent = transform;

                var isOffset = (x + y) % 2 == 1;
                spawnedField.Init(isOffset, disabledFields.Contains((x, y)), flagRegion.Contains((x, y)));
                fields.Add(spawnedField);
            }
        }
    }

    private Vector3 GetWorldspacePosition(int x, int y, int z = 0) {
        
        float _x = boardZeroReference.zeroReferenceX + x * tileWidth;
        float _y = boardZeroReference.zeroReferenceY + y * tileWidth;

        return new Vector3(_x, _y, z);
    }

    private Vector3 GetWorldspaceScale(int width, int height) {

        return new Vector3(.4f * tileWidth * width, .4f * tileWidth * height);
    }

    public void SetPiece(Chessman piece, int x, int y) {
        piece.transform.position = GetWorldspacePosition(x, y, -1);
        piece.transform.localScale = GetWorldspaceScale(1, 1);
    }

    public void ShowMoveIndicator(int x, int y) {
        GameObject moveIndicator = Instantiate(_moveIndicatorPrefab, GetWorldspacePosition(x, y), Quaternion.identity);
        moveIndicator.transform.parent = transform;
        moveIndicator.transform.localScale = new Vector3(tileWidth * 0.5f, tileWidth * 0.5f);
        moveIndicators.Add(moveIndicator);
    }

    public void RemoveMoveIndicators() {
        moveIndicators.RemoveAll(i => { Destroy(i); return true; }); // destory all move indicators and remove from list
    }

    public Chessman CreatePiece(string name, int x, int y) {
        Chessman cm = Instantiate(_chesspiecePrefab, new Vector3(0, 0, -1), Quaternion.identity);
        cm.name = name;
        cm.transform.parent = transform;
        cm.Init();
        return cm;
    }
}
