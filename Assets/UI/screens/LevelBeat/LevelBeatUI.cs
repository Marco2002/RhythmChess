using System;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelBeatUI : MonoBehaviour {
    [SerializeField] private UIDocument _uiDocument;
    public event Action OnNextLevelButtonClicked, OnLevelMenuButtonClicked, OnRetryButtonClicked;
    
    private VisualElement _root;
    private Label _levelLabel, _numberOfMovesLabel;
    private Label[] _starIconLabels, _starNumberLabels;
    
    public int Stars {
        set {
            if (value == 3) {
                _starIconLabels[0].RemoveFromClassList("level-beat-star-icon-locked");
                _starIconLabels[1].RemoveFromClassList("level-beat-star-icon-locked");
            } else if (value == 2) {
                _starIconLabels[0].RemoveFromClassList("level-beat-star-icon-locked");
                _starIconLabels[1].AddToClassList("level-beat-star-icon-locked");
            } else {
                _starIconLabels[0].AddToClassList("level-beat-star-icon-locked");
                _starIconLabels[1].AddToClassList("level-beat-star-icon-locked");
            }
        }
    }
    
    public int[] RequiredStars {
        set {
            _starNumberLabels[0].text = value[0].ToString();
            _starNumberLabels[1].text = value[1].ToString();
        }
    }
    
    public int NumberOfMoves {
        set {
            _numberOfMovesLabel.text = value.ToString();
        }
    }

    private void OnEnable() {
        _root = _uiDocument.rootVisualElement;

        _levelLabel = _root.Q<Label>("LabelLevel");
        _numberOfMovesLabel = _root.Q<Label>("LabelNumberOfMoves");
        _starNumberLabels = new Label[2];
        _starIconLabels = new Label[2];
        _starIconLabels[0] = _root.Q<Label>("LabelStarIcon1");
        _starIconLabels[1] = _root.Q<Label>("LabelStarIcon2");
        _starNumberLabels[0] = _root.Q<Label>("LabelStarNumber1");
        _starNumberLabels[1] = _root.Q<Label>("LabelStarNumber2");
        var nextLevelButton = _root.Q<Button>("ButtonNext");
        var levelMenuButton = _root.Q<IconButton>("ButtonLevelMenu");
        var retryButton = _root.Q<IconButton>("ButtonRetry");

        // nextLevelButton.clicked += () => OnNextLevelButtonClicked?.Invoke();
        levelMenuButton.OnClicked += () => OnLevelMenuButtonClicked?.Invoke();
        retryButton.OnClicked += () => OnRetryButtonClicked?.Invoke();
        
        _root.RegisterCallback<PointerDownEvent>(evt => {
            if (evt.target != levelMenuButton && evt.target != retryButton) {
                OnNextLevelButtonClicked?.Invoke();
            }
        });
    }

    public void Open() {
        _root.style.visibility = Visibility.Visible;
    }

    public void Close() {
        _root.style.visibility = Visibility.Hidden;
    }
}