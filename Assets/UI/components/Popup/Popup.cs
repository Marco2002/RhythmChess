using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

[UxmlElement]
public partial class Popup: VisualElement {
    private const int ANIMATION_TIME = 300;

    public Popup() {
        style.display = DisplayStyle.None;
    }

    public void Open() {
        style.display = DisplayStyle.Flex;
        transform.scale = new Vector3(0.5f, 0.5f, 1f);
        
        experimental.animation
            .Scale(1, ANIMATION_TIME)
            .Ease(Easing.OutBack)
            .Start();
    }
    
    public void Close() {
        experimental.animation
            .Scale(0.5f, ANIMATION_TIME)
            .Ease(Easing.InBack)
            .OnCompleted(() => style.display = DisplayStyle.None)
            .Start();
    }
}