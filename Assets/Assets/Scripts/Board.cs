using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour {
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private Field _fieldPrefab;
    [SerializeField] private ChessPiece _chessPiecePrefab;
    [SerializeField] private GameObject _moveIndicatorPrefab;

    [SerializeField] private int _width, _height;
    [SerializeField] private float _movementSpeed;
    
    private (float zeroReferenceX, float zeroReferenceY ) boardZeroReference;
    private float tileWidth;
    private float tileHeight;
    private List<Field> fields;

    private readonly List<GameObject> moveIndicators = new();

    public void Init(int width, int height, List<(int x, int y)> disabledFields, List<(int x, int y)> flagRegion) {
        if(fields != null) {
            foreach(var field in fields) {
                Destroy(field.gameObject);
            }
        }
        _width = width;
        _height = height;
        var boardWidth = 2 * 0.8f * _mainCamera.aspect * _mainCamera.orthographicSize;

        var fieldDimensions = new Vector3(1, 0.75f, 1); // manually set to the scale of the _fieldPrefab
        var relativeTileHeight = fieldDimensions.y / fieldDimensions.x;
        tileWidth = boardWidth / _width;
        Debug.Log("desired tile width: " + tileWidth);
        Debug.Log("actual tile width: " + fieldDimensions.x);
        tileHeight = relativeTileHeight * tileWidth;
        Debug.Log("tile height and width: " + tileHeight + " " + tileWidth);
        var tileScale = tileWidth / fieldDimensions.x;
        if(_height * tileHeight > _mainCamera.orthographicSize*0.7*2) {
            // board is too tall -> tileWidth needs to be set based on the screen height and not screen width
            tileHeight = 2 * 0.75f * _mainCamera.orthographicSize / (_height + relativeTileHeight);
            //                                                             ^ added to account for tile side
            tileWidth = tileHeight / relativeTileHeight;
        }
        boardZeroReference = (-(tileWidth * _width / 2) + tileWidth / 2, -(tileHeight * _height / 2) + tileHeight / 2); // the coordinates of the (0, 0) file in unity units
        fields = new List<Field>();
        
        for (var x = 0; x < _width; x++) {
            for (var y = 0; y < _height; y++) {
                if (disabledFields.Contains((x, y))) continue;
                var spawnedField = Instantiate(_fieldPrefab, GetWorldSpacePosition(x, y, 1), Quaternion.identity);
                
                spawnedField.transform.localScale = new Vector3(tileScale, tileScale, 1);
                spawnedField.name = $"Field {x} {y}";
                spawnedField.transform.parent = transform;
                
                var isOffset = (x + y) % 2 == 1;
                spawnedField.Init(isOffset, flagRegion.Contains((x, y)));
                fields.Add(spawnedField);
            }
        }
    }

    private Vector3 GetWorldSpacePosition(int x, int y, int z = 0) {
        
        var worldSpaceX = boardZeroReference.zeroReferenceX + x * tileWidth;
        var worldSpaceY = boardZeroReference.zeroReferenceY + y * tileHeight;

        return new Vector3(worldSpaceX, worldSpaceY, z);
    }

    private Vector3 GetWorldSpaceScale(int width, int height) {

        return new Vector3(.4f * tileWidth * width, .4f * tileWidth * height);
    }

    public void SetPiece(ChessPiece piece, int x, int y) {
        piece.transform.position = GetWorldSpacePosition(x, y, y);
        piece.transform.localScale = GetWorldSpaceScale(1, 1);
    }

    public void MovePiece(ChessPiece piece, int x, int y) {
        StartCoroutine(AnimatedMove(piece, x, y));
    }

    private IEnumerator AnimatedMove(Component piece, int x, int y) {
        var destination = GetWorldSpacePosition(x, y, y);
        while (piece.transform.position != destination) {
            piece.transform.position = Vector3.MoveTowards(piece.transform.position, destination, _movementSpeed * tileWidth * Time.deltaTime);
            // Wait a frame 
            yield return null;
        }
    }

    public void ShowMoveIndicator(int x, int y) {
        var moveIndicator = Instantiate(_moveIndicatorPrefab, GetWorldSpacePosition(x, y, -1), Quaternion.identity);
        moveIndicator.transform.parent = transform;
        moveIndicator.transform.localScale = new Vector3(tileWidth * 0.5f, tileWidth * 0.5f);
        moveIndicators.Add(moveIndicator);
    }

    public void RemoveMoveIndicators() {
        moveIndicators.RemoveAll(i => { Destroy(i); return true; }); // destroy all move indicators and remove from list
    }

    public ChessPiece CreatePiece(string name, int x, int y) {
        var chessPiece = Instantiate(_chessPiecePrefab, new Vector3(0, 0, -1), Quaternion.identity);
        chessPiece.name = name;
        chessPiece.transform.parent = transform;
        chessPiece.Init();
        return chessPiece;
    }
}
