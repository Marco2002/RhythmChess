using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class SwipeableTabs : MonoBehaviour {
    [SerializeField] private UIDocument _uiDocument;
    [SerializeField] private float _swipeThreshold = 0.2f;
    [SerializeField] private int _numberOfTabs = 3;
    [SerializeField] private float _easing = 0.5f;
    private VisualElement visualElement;
    private float swipeStartOffset;
    private bool pressed;
    private float currentPosition;
    

    private void Start() {
        var root = _uiDocument.rootVisualElement;
        
        visualElement = root.Q<VisualElement>("SwipeableTabs");

        if (visualElement == null) return;
        visualElement.RegisterCallback<PointerDownEvent>(OnPointerDown);
        visualElement.RegisterCallback<PointerUpEvent>(OnPointerUp);
        visualElement.RegisterCallback<PointerMoveEvent>(OnPointerMove);
    }
    
    private void OnPointerDown(PointerDownEvent evt) {
        pressed = true;
        swipeStartOffset = evt.position.x;
    }
    
    private void OnPointerMove(PointerMoveEvent evt) {
        if (!pressed) return;
        var swipeLength = (evt.position.x - swipeStartOffset) / (visualElement.layout.width / _numberOfTabs);
        visualElement.style.left = new Length((-currentPosition + swipeLength) * 100, LengthUnit.Percent);
    }
    
    private void OnPointerUp(PointerUpEvent evt) {
        pressed = false;
        var swipeLength = (evt.position.x - swipeStartOffset) / (visualElement.layout.width / _numberOfTabs);
        float newPosition = currentPosition;
        if (Math.Abs(swipeLength) >= _swipeThreshold) {
            newPosition += (swipeLength > 0 ? -1 : +1);
            newPosition = Math.Max(Math.Min(_numberOfTabs - 1, newPosition), 0);
        }
        StartCoroutine(LerpLeftPosition((-currentPosition + swipeLength) * 100, -newPosition * 100, _easing));
        currentPosition = newPosition;
    }
    
    IEnumerator LerpLeftPosition(float startPercent, float endPercent, float duration) {
        var elapsedTime = 0f;

        while (elapsedTime < duration) {
            var newLeft = Mathf.Lerp(startPercent, endPercent, elapsedTime / duration);
            visualElement.style.left = new Length(newLeft, LengthUnit.Percent);
            elapsedTime += Time.deltaTime;
            
            yield return null;
        }
        visualElement.style.left = new Length(endPercent, LengthUnit.Percent);
    }
}
