using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

[DefaultExecutionOrder(-1)]
public class InputManager : MonoBehaviour {

    #region Events
    public delegate void StartTouch(Vector2 position, float time);
    public event StartTouch OnStartTouch;
    public delegate void EndTouch(Vector2 position, float time);
    public event EndTouch OnEndTouch;
    #endregion
    private PlayerControls playerControls;

    private void Awake() {
        playerControls = new PlayerControls();
    }

    private void OnEnable() {
        playerControls.Enable();
    }
    private void OnDisable() {
        playerControls.Disable();
    }

    private void Start() {
        playerControls.Touch.PrimaryContact.started += StartTouchPrimary;
        playerControls.Touch.PrimaryContact.canceled += EndTouchPrimary;
    }

    private async void StartTouchPrimary(InputAction.CallbackContext context) {
        await Task.Delay(50); // this is needed, otherwise the first touch input is recorded wrong
        OnStartTouch?.Invoke(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.startTime);
    }

    private void EndTouchPrimary(InputAction.CallbackContext context) {
        OnEndTouch?.Invoke(playerControls.Touch.PrimaryPosition.ReadValue<Vector2>(), (float)context.time);
    }

    public Vector2 PrimaryPosition() {
        return playerControls.Touch.PrimaryPosition.ReadValue<Vector2>();
    }
}
