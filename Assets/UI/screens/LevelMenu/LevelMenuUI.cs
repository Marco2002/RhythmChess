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
    
    private IEnumerator OpenAnimation(bool reverse = false) {
       _root.style.visibility = Visibility.Visible;
       _mask.Init();
       const int duration = 300;
       const int startingWidth = 20;
       var targetWidth = _root.resolvedStyle.width + _root.resolvedStyle.height * 2;
       var startingTop = 24;
       var startingLeft = 30;
       var targetTop = startingTop - (targetWidth-startingWidth) / 2;
       var targetLeft = startingLeft - (targetWidth-startingWidth) / 2;
       var size = reverse ? new Vector2(startingWidth, startingWidth) : new Vector2(targetWidth, targetWidth);
       var position = reverse ? new Vector2(startingLeft, startingTop) : new Vector2(targetLeft, targetTop);

       yield return null;
       
       _mask.experimental.animation
          .Size(size, duration)
          .OnCompleted(() => {
             if(reverse) _root.style.visibility = Visibility.Hidden;
          }).Start();
       _mask.experimental.animation.TopLeft(position, duration).Start();
       yield return null;
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
        _mask.Init();
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
        
        StartCoroutine(OpenAnimation());
        Refresh();
     }
     
     public void Close() {
        StartCoroutine(OpenAnimation(true));
     }
}