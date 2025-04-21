using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class LevelButton : VisualElement {
    private Label labelStars;
    private Button button;
    public event Action OnClicked;
    
    [UxmlAttribute]
    public string LevelText {
        get => button.text;
        set => button.text = value;
    }

    [UxmlAttribute]
    public int NumberOfStars {
        get => labelStars.text.Length;
        set {
            labelStars.text = string.Concat(Enumerable.Repeat("\uf005", value));
            if (value == 3) {
                labelStars.style.color = new StyleColor(new Color(0.737f, 0.722f, 0.224f));
            }
        }
    }

    [UxmlAttribute]
    public bool Locked {
        get => button.ClassListContains("level-button_locked");
        set {
            if (value) {
                button.AddToClassList("level-button_locked");
                button.SetEnabled(false);
            } else {
                button.RemoveFromClassList("level-button_locked");
                button.SetEnabled(true);
            }
        }
    }

    public LevelButton() {
        var visualTree = Resources.Load<VisualTreeAsset>("UI/levelButton");
        visualTree.CloneTree(this);
        
        labelStars = this.Q<Label>("LabelStars");
        button = this.Q<Button>("Button");

        button.clicked += () => OnClicked?.Invoke();
    }
}