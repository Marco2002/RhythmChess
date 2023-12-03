using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Board : MonoBehaviour {
    [SerializeField] private Field _fieldPrefab;
    [SerializeField] private Chessman _chesspiecePrefab;
    [SerializeField] private GameObject _moveIndicatorPrefab;

    [SerializeField] private int _width, _height;
    [SerializeField] private float _relativeTileHeight;
    [SerializeField] private float _relativeTileSideHeight;
    [SerializeField] private float _movementSpeed;

    private (float zeroReferenceX, float zeroReferenceY ) boardZeroReference;
    private float tileWidth;
    private float tileHeight;
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
        float boardWidth = 2 * 0.8f * Camera.main.aspect * Camera.main.orthographicSize;
        tileWidth = boardWidth / _width;
        tileHeight = tileWidth * _relativeTileHeight;
        float uiHeaderHeight = 2*0.1f * Camera.main.orthographicSize;
        float tileSideHeight = _relativeTileSideHeight * tileWidth;
        if(_height * tileHeight + tileSideHeight > Camera.main.orthographicSize*0.7*2) {
            // board is too tall -> tileWidth needs to be set based on the screen height and not screen width
            tileHeight = 2 * 0.75f * Camera.main.orthographicSize / (_height + _relativeTileSideHeight);
            //                                                             ^ added to account for tile side
            tileWidth = tileHeight / _relativeTileHeight;
            tileSideHeight = _relativeTileSideHeight * tileWidth;
        }
        boardZeroReference = (-(tileWidth * _width / 2) + tileWidth / 2, -(tileHeight * _height / 2) + tileHeight / 2 + tileSideHeight/2 - uiHeaderHeight/2); // the coordinates of the (0, 0) file in unity units
        fields = new();

        for (int x = 0; x < _width; x++) {
            for (int y = 0; y < _height; y++) {
                if (disabledFields.Contains((x, y))) continue;
                var spawnedField = Instantiate(_fieldPrefab, GetWorldspacePosition(x, y, 1), Quaternion.identity);
                spawnedField.transform.localScale = new Vector3(tileWidth, tileHeight);
                // set up of the tile side
                spawnedField.transform.GetChild(0).transform.localPosition = new Vector3(0, -.5f - (_relativeTileSideHeight / 2f), 0);
                spawnedField.transform.GetChild(0).transform.localScale = new Vector3(1, _relativeTileSideHeight);
                
                spawnedField.name = $"Field {x} {y}";
                spawnedField.transform.parent = transform;

                GameObject side = spawnedField.transform.GetChild(0).gameObject;
                //side.transform.position = GetWorldspacePosition(x, y, 1) - Vector3
                
                var isOffset = (x + y) % 2 == 1;
                spawnedField.Init(isOffset, flagRegion.Contains((x, y)));
                fields.Add(spawnedField);
            }
        }
    }

    private Vector3 GetWorldspacePosition(int x, int y, int z = 0) {
        
        float _x = boardZeroReference.zeroReferenceX + x * tileWidth;
        float _y = boardZeroReference.zeroReferenceY + y * tileHeight;

        return new Vector3(_x, _y, z);
    }

    private Vector3 GetWorldspaceScale(int width, int height) {

        return new Vector3(.4f * tileWidth * width, .4f * tileWidth * height);
    }

    public void SetPiece(Chessman piece, int x, int y) {
        piece.transform.position = GetWorldspacePosition(x, y, -1);
        piece.transform.localScale = GetWorldspaceScale(1, 1);
    }

    public void MovePiece(Chessman piece, int x, int y) {
        StartCoroutine(AnimatedMove(piece, x, y));
    }

    private IEnumerator AnimatedMove(Chessman piece, int x, int y) {
        Vector3 destination = GetWorldspacePosition(x, y);
        while (piece.transform.position != destination) {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, destination, _movementSpeed * tileWidth * Time.deltaTime);
            // Wait a frame 
            yield return null;
        }
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
