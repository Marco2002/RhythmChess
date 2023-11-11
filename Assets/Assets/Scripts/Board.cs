using System.Collections.Generic;
using UnityEngine;


public class Board : MonoBehaviour {

    [SerializeField] private int _width, _height;
    [SerializeField] private Field _fieldPrefab;
    [SerializeField] private GameObject _moveIndicatorPrefab;

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
                var spawnedField = Instantiate(_fieldPrefab, GetWorldspacePosition(x, y, 1), Quaternion.identity);
                spawnedField.transform.localScale = new Vector3(tileWidth, tileWidth);
                spawnedField.name = $"Field {x} {y}";

                var isOffset = (x + y) % 2 == 1;
                spawnedField.Init(isOffset, disabledFields.Contains((x, y)), flagRegion.Contains((x, y)));
                fields.Add(spawnedField);
            }
        }
    }

    private Vector3 GetWorldspacePosition(int x, int y, int z = 0) {
        return new Vector3(-halfWidth + x * 2 * halfWidth / _width + tileWidth / 2, -(tileWidth * _height / 2) + y * 2 * halfWidth / _width + tileWidth / 2, z);
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
        moveIndicator.transform.localScale = new Vector3(tileWidth * 0.5f, tileWidth * 0.5f);
        moveIndicators.Add(moveIndicator);
    }

    public void RemoveMoveIndicators() {
        moveIndicators.RemoveAll(i => { Destroy(i); return true; }); // destory all move indicators and remove from list
    }
}
