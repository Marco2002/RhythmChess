using UnityEngine;

public class Background : MonoBehaviour {
    [SerializeField] private Camera _mainCamera;
    [SerializeField] private float _rotationSpeed;

    private SpriteRenderer spriteRenderer;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);
    }

    public void SetColoring(Coloring coloring) {
        _mainCamera.backgroundColor = coloring == Coloring.Primary ? ColorScheme.primary : ColorScheme.secondary;
        spriteRenderer.color = coloring == Coloring.Primary ? ColorScheme.primaryOffset : ColorScheme.secondaryOffset;
    }
}
