using System;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

[CreateAssetMenu(menuName = "2D/Tiles/Chessboard Rule Tile")]
public class ChessboardRuleTile : RuleTile {
    [SerializeField] private Sprite _shadowSprite;
    [SerializeField] private Sprite _shadowSpriteExtended;
    
    private GameObject shadow;

    public void AddShadow(Vector3Int position, ITilemap tilemap) {
        var isExtended = tilemap.GetSprite(position + new Vector3Int(0, 1, 0)) != null;
        shadow = new GameObject("Shadow");
        var tilemapComponent = tilemap.GetComponent<Tilemap>();
        shadow.transform.SetParent(tilemapComponent.gameObject.transform);
        shadow.transform.localScale = Vector3.one;
        shadow.transform.position = tilemapComponent.GetCellCenterWorld(position) + new Vector3(0, 0, 0);
        var sr = shadow.AddComponent<SpriteRenderer>();
        sr.sprite = isExtended ? _shadowSpriteExtended : _shadowSprite;
        sr.sortingOrder = -1;
        var color = sr.color;
        color.a = 0.3f;
        sr.color = color;
            
        var flatAlphaShader = Shader.Find("Custom/ShadowShader");
        var material = new Material(flatAlphaShader);
        sr.material = material;
    }

    public override bool RuleMatch(int neighbor, TileBase other) {
        if (neighbor == TilingRuleOutput.Neighbor.This) {
            return other != null;
        }
        if (neighbor == TilingRuleOutput.Neighbor.NotThis) {
            return other == null;
        }

        return base.RuleMatch(neighbor, other);
    }
}