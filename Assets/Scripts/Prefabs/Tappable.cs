using System;
using UnityEngine;

public class Tappable: MonoBehaviour {
    public Action OnTapped;
    void Update() {
        if (Input.touchCount > 0) {
            var touch = Input.GetTouch(0);

            if (touch.phase != TouchPhase.Began) return;
            Vector2 worldPoint = Camera.main.ScreenToWorldPoint(touch.position);
            RaycastHit2D hit = Physics2D.Raycast(worldPoint, Vector2.zero);

            if (hit.collider && hit.collider.gameObject == gameObject) {
                OnTapped?.Invoke();
            }
        }
    }
}