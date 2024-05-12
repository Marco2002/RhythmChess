using UnityEngine;

public class Field : MonoBehaviour {
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _sideRenderer;
    [SerializeField] private SpriteRenderer _gradient;
    [SerializeField] private SpriteRenderer _light;
    public void Init(bool isOffset, bool isFlagRegion, bool isTopTile) {
        Color color;
        if (isOffset) {
            color = isFlagRegion ? ColorScheme.fieldFlagOffset : ColorScheme.fieldOffset;
            _gradient.color = Color.clear;
        } else {
            color = isFlagRegion ? ColorScheme.fieldFlag : ColorScheme.field;
            if(isFlagRegion) _gradient.color = Color.clear;
        }

        if (!isTopTile) _light.color = Color.clear;
        //var sideColor = new Color(color.r * .8f, color.g * .8f, color.b * .8f);
        _renderer.color = color;
        _sideRenderer.color = color;
    }
}
