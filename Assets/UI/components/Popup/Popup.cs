using System;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

[UxmlElement]
public partial class Popup: VisualElement {
    private const int ANIMATION_TIME = 300;
    private VisualElement _container;
    public event Action OnTappedOutside;

    public Popup() {
        _container = new VisualElement();
        _container.name = "popup content";
        style.position = Position.Absolute;
        style.top = 0;
        style.left = 0;
        style.right = 0;
        style.bottom = 0;
        style.justifyContent = Justify.Center;
        style.backgroundColor = new StyleColor(new Color(0, 0, 0, 0.4f));
        _container.style.alignSelf = Align.Center;
        hierarchy.Add(_container);
        style.display = DisplayStyle.None;
        
        RegisterCallback<PointerDownEvent>(evt => {
            if (!_container.worldBound.Contains(evt.position)) {
                OnTappedOutside?.Invoke();
            }
        });
    }

    public override VisualElement contentContainer => _container;

    public void Open() {
        style.display = DisplayStyle.Flex;
        _container.transform.scale = new Vector3(0.5f, 0.5f, 1f);
        
        _container.experimental.animation
            .Scale(1, ANIMATION_TIME)
            .Ease(Easing.OutBack)
            .Start();
    }
    
    public void Close() {
        _container.experimental.animation
            .Scale(0.5f, ANIMATION_TIME)
            .Ease(Easing.InBack)
            .OnCompleted(() => style.display = DisplayStyle.None)
            .Start();
    }
}