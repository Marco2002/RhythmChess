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

    public void SetPrimary() {
        _mainCamera.backgroundColor = ColorScheme.primary;
        spriteRenderer.color = ColorScheme.primaryOffset;
    }
    
    public void SetSecondary() {
        _mainCamera.backgroundColor = ColorScheme.secondary;
        spriteRenderer.color = ColorScheme.secondaryOffset;
    }

}
