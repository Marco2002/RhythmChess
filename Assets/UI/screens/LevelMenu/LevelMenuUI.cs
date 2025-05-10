using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;

public class LevelMenuUI : MonoBehaviour {
    [SerializeField] private UIDocument _uiDocument;
    private VisualElement _root;
    private LevelButton[] _buttons;
    public event Action OnCloseButtonClicked;
    public event Action<int> OnLevelButtonClicked;
     private void OnEnable() {
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
        Refresh();
     }
     
     public void Close() {
        _root.style.visibility = Visibility.Hidden;
     }
}