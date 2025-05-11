using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Mask : VisualElement {
    private VisualElement _container;
    public Mask() {
        // Set up the internal container
        _container = new VisualElement();
        _container.name = "mask content";
        hierarchy.Add(_container);
        style.overflow = Overflow.Hidden;
        RegisterCallback<GeometryChangedEvent>(OnGeometryChanged);
    }

    public override VisualElement contentContainer => _container;

    private void OnGeometryChanged(GeometryChangedEvent evt) {
        var parentElement = parent;
        if (parentElement != null) {
            _container.style.width = parentElement.resolvedStyle.width;
            _container.style.height = parentElement.resolvedStyle.height;
        }
        _container.style.marginTop = -resolvedStyle.top;
        _container.style.marginLeft = -resolvedStyle.left;
    }
}