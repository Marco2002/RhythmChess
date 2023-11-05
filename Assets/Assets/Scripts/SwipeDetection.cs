using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwipeDetection : MonoBehaviour
{
    [SerializeField] private InputManager inputManager;

    [SerializeField] private float minimumDistance = .2f;
    [SerializeField] private float maximumTime = 1f;

    private Vector2 startPos;
    private float startTime;
    private Vector2 endPos;
    private float endTime;
    public void OnEnable() {
        inputManager.OnStartTouch += SwipeStart;
        inputManager.OnEndTouch += SwipeEnd;
    }

    public void OnDisable() {
        inputManager.OnStartTouch -= SwipeStart;
        inputManager.OnEndTouch -= SwipeEnd;
    }

    private void SwipeStart(Vector2 position, float time) {
        startPos = position;
        startTime = time;
    }

    private void SwipeEnd(Vector2 position, float time) {
        endPos = position;
        endTime = time;
        DetectSwipe();
    }

    private void DetectSwipe() {
        if (Vector3.Distance(startPos, endPos) >= minimumDistance && endTime - startTime < maximumTime) {
            Debug.DrawLine(startPos, endPos, Color.red, 5f);
            Vector3 direction = endPos - startPos;
            Vector2 direction2D = new Vector2(direction.x, direction.y).normalized;
            gameObject.GetComponent<Controller>().RecordMove(direction2D);
        }
    }
}
