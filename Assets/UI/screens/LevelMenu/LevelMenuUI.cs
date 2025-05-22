using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelMenuUI : MonoBehaviour {
    private UIDocument _uiDocument;
    private VisualElement _root;
    private LevelButton[] _buttons;
    private Mask _mask;
    private int _activeLevel;
    public event Action OnCloseButtonClicked;
    public event Action<int> OnLevelButtonClicked;
    
    void AnimateButtonPulse(int index) {
       var duration = 400;
       var targetScale = 1.2f;
          
       void Animate(bool expanding) {
          if (_activeLevel != index) {
             _buttons[index].style.scale = Vector2.one;
             return;
          }
          
          var target = expanding ? targetScale : 1f;
          _buttons[index].experimental.animation
             .Scale(target, duration)
             .OnCompleted(() => {
                Animate(!expanding);
             }).Start();
       }

       Animate(true);
    }

    public int ActiveLevel {
        get => _activeLevel;
        set {
            _activeLevel = value;
            AnimateButtonPulse(value);
        }
    }
    
    private IEnumerator AnimateMask(bool reverse = false) {
       const float duration = 0.3f;
       var elapsedTime = 0f;
       const int startingWidth = 20;
       const int startingTop = 24;
       const int startingLeft = 30;
       var targetWidth = _root.resolvedStyle.width + _root.resolvedStyle.height * 2;

       while (elapsedTime < duration) {
          var lerp = reverse ? Mathf.Lerp(targetWidth, startingWidth, elapsedTime / duration) 
             : Mathf.Lerp(startingWidth, targetWidth, elapsedTime / duration);
          _mask.style.width = lerp;
          _mask.style.height = lerp;
          _mask.style.top = reverse ? Mathf.Lerp(startingTop - targetWidth/2, startingTop, elapsedTime / duration)
             : Mathf.Lerp(startingTop, startingTop - targetWidth/2,elapsedTime / duration);
          _mask.style.left = reverse ? Mathf.Lerp(startingLeft - targetWidth/2, startingLeft, elapsedTime / duration)
             : Mathf.Lerp(startingLeft, startingLeft - targetWidth/2,elapsedTime / duration);
          elapsedTime += Time.deltaTime;
          yield return null;
       }

       if(reverse) {
          _root.style.visibility = Visibility.Hidden;
       } else {
          _mask.style.left = startingLeft - targetWidth/2;
          _mask.style.top = startingTop - targetWidth/2;
          _mask.style.width = targetWidth;
          _mask.style.height = targetWidth;
       }
    }
    
     private void OnEnable() {
        _uiDocument = GetComponent<UIDocument>();
        _root = _uiDocument.rootVisualElement;
        
        var closeButton = _root.Q<Button>("ButtonClose");
        closeButton.clicked += () => {
           OnCloseButtonClicked?.Invoke();
           Close();
        };
        var levelList = _root.Q<VisualElement>("LevelList");
        
        _buttons = new LevelButton[GameConstants.NUMBER_OF_LEVELS];
        for (int i = 0; i < GameConstants.NUMBER_OF_LEVELS; i++) {
           _buttons[i] = new LevelButton() { LevelText = $"{i+1}" };
           var i1 = i;
           _buttons[i].OnClicked += () => {
              Close();
              OnLevelButtonClicked?.Invoke(i1);
           };
           levelList.Add(_buttons[i]);
        }

        _mask = _root.Q<Mask>("Mask");
     }

     private void Refresh() {
        var currentLevel = PlayerPrefs.GetInt("currentLevel");
        var levelStatus = PlayerPrefs.GetString("levelStatus")
           .Select(c => int.Parse(c.ToString())).ToArray();
        for (var i = 0; i < GameConstants.NUMBER_OF_LEVELS; i++) {
           _buttons[i].Locked = i > currentLevel;
           _buttons[i].NumberOfStars = levelStatus[i];
        }
     }
     
     public void Open() {
        _root.style.visibility = Visibility.Visible;
        StartCoroutine(AnimateMask());
        Refresh();
     }
     
     public void Close() {
        StartCoroutine(AnimateMask(true));
     }
}