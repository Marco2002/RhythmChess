using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.UIElements.Experimental;

public class LevelBeatUI : MonoBehaviour {
    private UIDocument _uiDocument;
    private SpriteRenderer _background;

    public event Action OnNextLevelButtonClicked, OnLevelMenuButtonClicked, OnRetryButtonClicked;
    
    private VisualElement _root;
    private float worldScreenHeight;
    private Label _levelLabel, _numberOfMovesLabel;
    private Label[] _starIconLabels, _starNumberLabels;

    public int stars = 1;
    
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
        _uiDocument = GetComponent<UIDocument>();
        _root = _uiDocument.rootVisualElement;
        _background = GetComponent<SpriteRenderer>();
                
        worldScreenHeight = Camera.main.orthographicSize * 2;
        float worldScreenWidth = worldScreenHeight * Screen.width / Screen.height;

        Vector2 spriteSize = _background.sprite.bounds.size;

        transform.localScale = new Vector3(
            worldScreenWidth / spriteSize.x,
            worldScreenWidth / spriteSize.x,
            1);
        
        transform.position = new Vector3(0, -worldScreenHeight / 2, 0);

        _levelLabel = _root.Q<Label>("LabelLevel");
        _numberOfMovesLabel = _root.Q<Label>("LabelNumberOfMoves");
        _starNumberLabels = new Label[2];
        _starIconLabels = new Label[3];
        _starIconLabels[0] = _root.Q<Label>("LabelStarIcon0");
        _starIconLabels[1] = _root.Q<Label>("LabelStarIcon1");
        _starIconLabels[2] = _root.Q<Label>("LabelStarIcon2");
        _starNumberLabels[0] = _root.Q<Label>("LabelStarNumber1");
        _starNumberLabels[1] = _root.Q<Label>("LabelStarNumber2");
        var nextLevelButton = _root.Q<Button>("ButtonNext");
        var levelMenuButton = _root.Q<IconButton>("ButtonLevelMenu");
        var retryButton = _root.Q<IconButton>("ButtonRetry");

        nextLevelButton.clicked += () => OnNextLevelButtonClicked?.Invoke();
        levelMenuButton.OnClicked += () => OnLevelMenuButtonClicked?.Invoke();
        retryButton.OnClicked += () => OnRetryButtonClicked?.Invoke();
        
        _root.RegisterCallback<PointerDownEvent>(evt => {
            if (evt.target != levelMenuButton && evt.target != retryButton) {
                OnNextLevelButtonClicked?.Invoke();
            }
        });
    }

    public void Open() {
        StartCoroutine(OpenAnimation());
    }

    public void Close() {
        StartCoroutine(CloseAnimation());
    }
    
    private IEnumerator OpenAnimation() {
        var startPosition = new Vector3(0, -worldScreenHeight / 2, 0);
        var targetPosition = new Vector3(0, +worldScreenHeight / 2 + 2, 0);
        float slideDuration = 0.5f;
        float starPopDuration = 0.125f;
        float elapsedTime = 0;
        var filledStars = 0;

        for (var i = 0; i < 3; i++) {
            _starIconLabels[i].AddToClassList("level-beat-star-icon-locked");
            _starIconLabels[i].transform.scale = new Vector3(0.7f, 0.7f, 1f);
        }

        transform.rotation = Quaternion.Euler(0, 0, 0);

        while (elapsedTime < slideDuration) {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        transform.position = targetPosition;
        _root.style.visibility = Visibility.Visible;

        while (elapsedTime < slideDuration + starPopDuration * stars) {
            if (elapsedTime >  slideDuration + starPopDuration * filledStars) {
                _starIconLabels[filledStars].experimental.animation
                    .Scale(1f, (int)(starPopDuration * 1000))
                    .Ease(Easing.InBounce)
                    .Start();
                _starIconLabels[filledStars].RemoveFromClassList("level-beat-star-icon-locked");
                filledStars++;
            }
            elapsedTime += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator CloseAnimation() {
        var startPosition = new Vector3(0, -worldScreenHeight / 2 - 2, 0);
        var targetPosition = new Vector3(0, +worldScreenHeight / 2, 0);
        float slideDuration = 0.5f;
        float elapsedTime = 0;
        
        _root.style.visibility = Visibility.Hidden;
        transform.rotation = Quaternion.Euler(0, 0, 180);

        while (elapsedTime < slideDuration) {
            transform.position = Vector3.Lerp(startPosition, targetPosition, elapsedTime / slideDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        transform.position = targetPosition;
    }
}