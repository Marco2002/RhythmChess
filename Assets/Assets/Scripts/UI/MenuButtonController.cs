using UnityEngine;

public class MenuButtonController : MonoBehaviour {
    public CameraTransition cameraTransition;

    public void OnMenuButtonClicked() {
        cameraTransition.ToggleMenu();
    }
}