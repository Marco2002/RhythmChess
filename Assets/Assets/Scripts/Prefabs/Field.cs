using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Field : MonoBehaviour {
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _sideRenderer;

    private Mesh mesh;

    public void Init(bool isOffset, bool isFlagRegion) {
        if (isFlagRegion) {
            var color = ColorScheme.fieldFlag;
            _renderer.color = color;
            _sideRenderer.color = color;
        } else {
            if (isOffset) {
                _renderer.color = ColorScheme.fieldOffset;
                _sideRenderer.color = ColorScheme.fieldOffset;
            } else {
                _renderer.color = ColorScheme.field;
                _sideRenderer.color = ColorScheme.field;
            }
        }
        
        mesh = new Mesh();
        GetComponentInChildren<MeshFilter>().mesh = mesh;
    }
}
