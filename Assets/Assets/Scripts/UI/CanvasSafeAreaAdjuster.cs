using UnityEngine;

public class CanvasSafeAreaAdjuster : MonoBehaviour {
    [SerializeField] private RectTransform canvasTransform;

    void Start() {
        AdjustElementsToSafeArea();
    }

    void AdjustElementsToSafeArea() {
        Rect safeArea = Screen.safeArea; // Get the safe area of the screen
        Vector2 anchorMin = safeArea.position + new Vector2(20, 20);
        Vector2 anchorMax = safeArea.position + safeArea.size - new Vector2(20, 20);

        // Convert from absolute pixel coordinates to normalized coordinates (0 to 1)
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;
        
        canvasTransform.anchorMin = anchorMin;
        canvasTransform.anchorMax = anchorMax;
        canvasTransform.offsetMax = Vector2.zero;
        canvasTransform.offsetMin = Vector2.zero;
    }
}