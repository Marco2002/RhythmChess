using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class HintCard: VisualElement {
    private VisualElement image;
    private Sprite _imageSprite;
    private Label label;

    [UxmlAttribute]
    public Sprite Image {
        get => _imageSprite;
        set {
            _imageSprite = value;
            image.style.backgroundImage = value != null ? Background.FromSprite(value) : StyleKeyword.None;
        }
    }
    
    [UxmlAttribute]
    public string Label {
        get => label.text;
        set => label.text = value;
    }

    public HintCard() {
        var visualTree = Resources.Load<VisualTreeAsset>("UI/hintCard");
        visualTree.CloneTree(this);
        image = this.Q<VisualElement>("Image");
        label = this.Q<Label>("Label");
    }
}