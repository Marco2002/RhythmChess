using UnityEngine;

public class Field : MonoBehaviour {
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _sideRenderer;

    public void Init(bool isOffset, bool isFlagRegion) {
        Color color;
        if(isFlagRegion) {
            color = isOffset ? ColorScheme.fieldFlagOffset : ColorScheme.fieldFlag;
        } else { 
            color = isOffset ? ColorScheme.fieldOffset : ColorScheme.field;
        }
        //var sideColor = new Color(color.r * .8f, color.g * .8f, color.b * .8f);
        _renderer.color = color;
        _sideRenderer.color = color;
    }
}
