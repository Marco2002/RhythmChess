using UnityEngine;

public class CanvasSafeAreaAdjuster : MonoBehaviour {
    [SerializeField] private RectTransform buttonRectTransform; // The RectTransform of the TMP button
    [SerializeField] private RectTransform titleTransform;

    void Start() {
        AdjustElementsToSafeArea();
    }

    void AdjustElementsToSafeArea() {
        Rect safeArea = Screen.safeArea; // Get the safe area of the screen
        Vector2 anchorMin = safeArea.position + new Vector2(20, 80);
        Vector2 anchorMax = safeArea.position + safeArea.size - new Vector2(20, 80);

        // Convert from absolute pixel coordinates to normalized coordinates (0 to 1)
        anchorMin.x /= Screen.width;
        anchorMin.y /= Screen.height;
        anchorMax.x /= Screen.width;
        anchorMax.y /= Screen.height;

        // Adjust the button's position based on the safe area
        buttonRectTransform.anchorMin = new Vector2(anchorMin.x, anchorMax.y); // Top-left
        buttonRectTransform.anchorMax = new Vector2(anchorMin.x, anchorMax.y); // Top-left
        
        // Adjust the title's position based on the safe area
        titleTransform.anchorMin = new Vector2(anchorMin.x +(anchorMax.x - anchorMin.x) / 2, anchorMax.y); // Top-center
        titleTransform.anchorMax = new Vector2(anchorMin.x +(anchorMax.x - anchorMin.x) / 2, anchorMax.y); // Top-center
    }
}