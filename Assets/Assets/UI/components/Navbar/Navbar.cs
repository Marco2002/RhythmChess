using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Navbar : VisualElement {
    private Label labelLevel, labelTitle;
    private Button buttonLevels, buttonSettings;
    private ProgressBar progressBarLeft, progressBarRight;
    private VisualElement progressBarNode;
    private float progress;
    public event Action OnSettingsButtonClicked, OnLevelsButtonClicked;


    [UxmlAttribute]
    public string LevelText {
        get => labelLevel.text;
        set => labelLevel.text = value;
    }
    
    [UxmlAttribute]
    public string TitleText {
        get => labelTitle.text;
        set => labelTitle.text = value;
    }

    [UxmlAttribute]
    public float Progress {
        get => progress;
        set {
            progress = value;
            progressBarLeft.value = Math.Min(100, value * 2);
            progressBarRight.value = Math.Max(0, (value - 50) * 2);
            progressBarNode.style.backgroundColor = new StyleColor(value < 50 ? new Color(44 / 255f, 47 / 255f, 54 / 255f) : Color.white);

            if (progress < 50) progressBarRight.AddToClassList("progress-zero");
            else progressBarRight.RemoveFromClassList("progress-zero");
        }
    }

    public Navbar() {
        var visualTree = Resources.Load<VisualTreeAsset>("UI/navbar");
        visualTree.CloneTree(this);
        
        progressBarLeft = this.Q<ProgressBar>("ProgressBarLeft");
        progressBarRight = this.Q<ProgressBar>("ProgressBarRight");
        progressBarNode = this.Q<VisualElement>("ProgressBarNode");
        buttonLevels = this.Q<Button>("ButtonLevels");
        buttonSettings = this.Q<Button>("ButtonSettings");
        labelLevel = this.Q<Label>("LabelLevel");
        labelTitle = this.Q<Label>("LabelTitle");
        
        progressBarNode.style.backgroundColor = new StyleColor(new Color(44 / 255f, 47 / 255f, 54 / 255f));
        buttonLevels.clicked += () => OnLevelsButtonClicked.Invoke();
        buttonSettings.clicked += () => OnSettingsButtonClicked.Invoke();
    }
}