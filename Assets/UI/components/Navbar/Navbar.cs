using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class Navbar : VisualElement {
    private Label labelLevel, labelTitle;
    private Button buttonLevels, buttonSettings;
    private VisualElement nodesContainer;
    private VisualElement[] nodes;
    private float progress;
    private int _numberOfEnabledNodes;
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
    public int Nodes {
        get => nodes?.Length ?? 0;
        set {
            if (nodes != null) {
                foreach (var node in nodes) {
                    nodesContainer.Remove(node);
                }
            }
            nodes = new VisualElement[value];
            for (var i = 0; i < value; i++) {
                var node = new VisualElement();
                node.AddToClassList("navbar-node");
                nodes[i] = node;
                nodesContainer.Add(node);
            }
        }
    }
    
    [UxmlAttribute]
    public int NodesEnabled {
        get => _numberOfEnabledNodes;
        set {
            if (value > nodes.Length) return;
            for (var i = 0; i < nodes.Length; i++) {
                nodes[i].EnableInClassList("navbar-node-active", i < value);
            }
            _numberOfEnabledNodes = value;
        }
    }

    public Navbar() {
        var visualTree = Resources.Load<VisualTreeAsset>("UI/navbar");
        visualTree.CloneTree(this);
        
        nodesContainer = this.Q<VisualElement>("NodesContainer");
        buttonLevels = this.Q<Button>("ButtonLevels");
        buttonSettings = this.Q<Button>("ButtonSettings");
        labelLevel = this.Q<Label>("LabelLevel");
        labelTitle = this.Q<Label>("LabelTitle");
        
        buttonLevels.clicked += () => OnLevelsButtonClicked.Invoke();
        buttonSettings.clicked += () => OnSettingsButtonClicked.Invoke();
    }
}