using UnityEngine;

public class CameraTransition : MonoBehaviour {
    private readonly Vector3 gameAreaPosition = new(0, 0, -10); // Position where the game happens
    private readonly Vector3 menuPosition = new(0, 10, -10); // Position where the menu is
    [SerializeField] private float transitionSpeed; // Speed of the transition
    private bool moveToMenu = false;

    private void Update() {
        if (moveToMenu) {
            transform.position = Vector3.Lerp(transform.position, menuPosition, Time.deltaTime * transitionSpeed);
        } else {
            transform.position = Vector3.Lerp(transform.position, gameAreaPosition, Time.deltaTime * transitionSpeed);
        }
    }
    
    public void ToggleMenu() {
        moveToMenu = !moveToMenu; // Toggle between game and menu
    }
}