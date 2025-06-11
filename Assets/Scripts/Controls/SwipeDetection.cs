using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] private InputManager _inputManager;
    [SerializeField] private Controller _controller;

    [SerializeField] private float _minimumDistance = .4f;
    [SerializeField] private float _maximumTime = 1f;

    private Vector2 startPosition;
    private float startTime;
    private Vector2 endPosition;
    private float endTime;
    private bool inTouch;

    private void Awake() {
        _controller = gameObject.GetComponent<Controller>();
    }

    public void OnEnable() {
        _inputManager.OnStartTouch += SwipeStart;
        _inputManager.OnEndTouch += SwipeEnd;
    }

    public void OnDisable() {
        _inputManager.OnStartTouch -= SwipeStart;
        _inputManager.OnEndTouch -= SwipeEnd;
    }

    private void SwipeStart(Vector2 position, float time) {
        startPosition = position;
        startTime = time;
        inTouch = true;
    }

    private void SwipeEnd(Vector2 position, float time) {
        endPosition = position;
        endTime = time;
        DetectSwipe();
        inTouch = false;
    }

    private void DetectSwipe() {
        if (!(Vector3.Distance(startPosition, endPosition) >= _minimumDistance) ||
            !(endTime - startTime < _maximumTime)) return;
        Debug.DrawLine(startPosition, endPosition, Color.red, 5f);
        Vector3 direction = endPosition - startPosition;
        var direction2D = new Vector2(direction.x, direction.y).normalized;
        gameObject.GetComponent<Controller>().RecordMove(direction2D);

    }
 

    public void DetectSwipeForCurrentTouch() {
        if (!inTouch) return;
        var currentPosition = _inputManager.PrimaryPosition();
        if (Vector3.Distance(startPosition, currentPosition) >= _minimumDistance) {
            Vector3 direction = currentPosition - startPosition;
            var direction2D = new Vector2(direction.x, direction.y).normalized;
            _controller.RecordMove(direction2D);
        }
        inTouch = false;
    }

}
