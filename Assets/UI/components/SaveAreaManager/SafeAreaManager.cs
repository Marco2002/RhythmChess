using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SafeAreaManager : VisualElement {

    public SafeAreaManager() {
        style.flexGrow = 1;
        style.flexShrink = 1;
        RegisterCallback<GeometryChangedEvent>(LayoutChanged);
    }

    private void LayoutChanged(GeometryChangedEvent e) {
        var safeArea = Screen.safeArea;

        try {
            var leftTop = RuntimePanelUtils.ScreenToPanel(panel,
                new Vector2(safeArea.xMin, Screen.height - safeArea.yMax));
            var rightBottom = RuntimePanelUtils.ScreenToPanel(panel,
                new Vector2(Screen.width - safeArea.xMax, safeArea.yMin));

            style.marginLeft = leftTop.x;
            style.marginTop = leftTop.y;
            style.marginRight = rightBottom.x;
            style.marginBottom = rightBottom.y;
        } catch (InvalidCastException) {
        }
    }
}