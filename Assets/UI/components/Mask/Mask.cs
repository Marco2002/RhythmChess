using UnityEngine.UIElements;

[UxmlElement]
public partial class Mask : VisualElement {
    private VisualElement _container;
    public Mask() {
        _container = new VisualElement();
        _container.name = "mask content";
        _container.AddToClassList("mask-content");
        _container.style.position = Position.Absolute;
        
        hierarchy.Add(_container);
        style.overflow = Overflow.Hidden;
    }

    public void Init() {
        var parentElement = parent;
        if (parentElement == null || _container.style.width == parentElement.resolvedStyle.width) return;
        _container.style.width = parentElement.resolvedStyle.width;
        _container.style.height = parentElement.resolvedStyle.height;
    }

    public override VisualElement contentContainer => _container;
}