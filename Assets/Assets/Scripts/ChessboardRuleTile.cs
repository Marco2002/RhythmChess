using System.Numerics;
using UnityEngine;
using UnityEngine.Tilemaps;
using Vector3 = UnityEngine.Vector3;

[CreateAssetMenu(menuName = "2D/Tiles/Chessboard Rule Tile")]
public class ChessboardRuleTile : RuleTile {
    [SerializeField] private Sprite shadowSprite;
    [SerializeField] private Sprite shadowSpriteExtended;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData) {
        base.GetTileData(position, tilemap, ref tileData);

        if (this == null || shadowSprite == null) return;
        
        var shadow = new GameObject("Shadow");
        var isExtended = tilemap.GetSprite(position + new Vector3Int(0, 1, 0)) != null;
        var tilemapComponent = tilemap.GetComponent<Tilemap>();
        shadow.transform.SetParent(tilemapComponent.gameObject.transform);
        shadow.transform.localScale = Vector3.one;
        shadow.transform.position = tilemapComponent.GetCellCenterWorld(position) + new Vector3(0, 0, 0);
        var sr = shadow.AddComponent<SpriteRenderer>();
        sr.sprite = isExtended ? shadowSpriteExtended : shadowSprite;
        sr.sortingOrder = tileData.sprite ? -1 : -2;
        
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