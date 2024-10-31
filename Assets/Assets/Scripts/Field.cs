using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Field : MonoBehaviour {
    [SerializeField] private SpriteRenderer _renderer;
    [SerializeField] private SpriteRenderer _sideRenderer;
    [SerializeField] private SpriteRenderer _gradient;
    [SerializeField] private SpriteRenderer _light;
    [SerializeField] private float _borderWidth;
    [SerializeField] private Color _borderColor = Color.black;

    private Mesh mesh;
    private bool isOffset;
    private bool isFlagRegion;

    public void Init(bool isOffset, bool isFlagRegion, bool isTopTile) {
        this.isOffset = isOffset;
        this.isFlagRegion = isFlagRegion;
        if (!isTopTile) _light.color = Color.clear;
        if (isOffset || isFlagRegion) _gradient.color = Color.clear;
        
        if (isFlagRegion) {
            var color = isOffset ? ColorScheme.fieldFlagOffset : ColorScheme.fieldFlag;
            _renderer.color = color;
            _sideRenderer.color = color;
            // TODO find a solution for two adjacent flag fields (the border looks wired between them
        } else {
            SetColoring(Coloring.Primary);
        }
        
        mesh = new Mesh();
        GetComponentInChildren<MeshFilter>().mesh = mesh;
        CreateBorder();
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
    
    private void CreateBorder() {
        // Define the vertices for the outer and inner border (quad-shaped border)
        var vertices = new Vector3[16];
        const float outer = 1f / 2f;
        var inner = outer - _borderWidth;
        // Outer square vertices (clockwise)
        vertices[0] = new Vector3(-outer, outer);    // Top left
        vertices[1] = new Vector3(outer, outer);     // Top right
        vertices[2] = new Vector3(outer, -outer);    // Bottom right
        vertices[3] = new Vector3(-outer, -outer);   // Bottom left
        // Inner square vertices (clockwise)
        vertices[4] = new Vector3(-inner, inner);    // Top left
        vertices[5] = new Vector3(inner, inner);     // Top right
        vertices[6] = new Vector3(inner, -inner);    // Bottom right
        vertices[7] = new Vector3(-inner, -inner);   // Bottom left
        // Duplicate vertices to define the two parts of the border
        for (int i = 0; i < 8; i++) vertices[i + 8] = vertices[i];
        // Define triangles (2 triangles per quad)
        int[] triangles = {
            0, 4, 5,    0, 5, 1,   // Top border
            1, 5, 6,    1, 6, 2,   // Right border
            2, 6, 7,    2, 7, 3,   // Bottom border
            3, 7, 4,    3, 4, 0    // Left border
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        var meshRenderer = GetComponentInChildren<MeshRenderer>();
        
        Material material = new Material(Shader.Find("GUI/Text Shader")); // Create a new material with the shader
        material.color = _borderColor; // Set the color
        meshRenderer.material = material; 
        meshRenderer.material.color = _borderColor;
    }
}
