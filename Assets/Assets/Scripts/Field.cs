using UnityEngine;

public class Field : MonoBehaviour {
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _sideRenderer;
    [SerializeField] private SpriteRenderer _gradient;
    [SerializeField] private SpriteRenderer _light;
    
    private bool isOffset;
    private bool isFlagRegion;

    public void Init(bool isOffset, bool isFlagRegion, bool isTopTile) {
        this.isOffset = isOffset;
        this.isFlagRegion = isFlagRegion;
        if (!isTopTile) _light.color = Color.clear;
        if (isOffset) _gradient.color = Color.clear;
        
        if (isFlagRegion) {
            var color = isOffset ? ColorScheme.fieldFlagOffset : ColorScheme.fieldFlag;
            _renderer.color = color;
            _sideRenderer.color = color;
        } else {
            SetColoring(Coloring.Primary);
        }
    }

    public void SetColoring(Coloring coloring) {
        if (isFlagRegion) return;
        
        if (coloring == Coloring.Primary) {
            if (isOffset) {
                _renderer.color = ColorScheme.fieldPrimaryOffset;
                _sideRenderer.color = ColorScheme.fieldPrimaryOffset;
            } else {
                _renderer.color = ColorScheme.fieldPrimary;
                _sideRenderer.color = ColorScheme.fieldPrimary;
            }
        } else {
            if (isOffset) {
                _renderer.color = ColorScheme.fieldSecondaryOffset;
                _sideRenderer.color = ColorScheme.fieldSecondaryOffset;
            } else {
                _renderer.color = ColorScheme.fieldSecondary;
                _sideRenderer.color = ColorScheme.fieldSecondary;
            }
        }
    }
}
