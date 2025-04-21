using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class IconButton : VisualElement {
    private Button button;
    public event Action OnClicked;
    
    [UxmlAttribute]
    public string IconGlyph {
        get => button.text;
        set => button.text = value;
    }

    public IconButton() {
        var visualTree = Resources.Load<VisualTreeAsset>("UI/iconButton");
        visualTree.CloneTree(this);
        button = this.Q<Button>("IconButton");

        button.clicked += () => OnClicked?.Invoke();
    }
}