using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour {
    [SerializeField] private Button _levelButtonPrefab;
    [SerializeField] private Transform _buttonContainer;
    [SerializeField] private int _totalLevels = 10;
    [SerializeField] public Controller _controller;

    private Button[] _buttons;
    private void Start() {
        _buttons = new Button[_totalLevels];
        for (var i = 0; i < _totalLevels; i++) {
            var levelNumber = i+1;
            var button = Instantiate(_levelButtonPrefab, _buttonContainer);
            _buttons[i] = button;
            button.GetComponentInChildren<Text>().text = "Level " + levelNumber;
            button.onClick.AddListener(() => LoadLevel(levelNumber));
        }
        Refresh();
    }

    private void LoadLevel(int levelIndex) {
        _controller.LoadLevel(levelIndex);
        gameObject.SetActive(false);
    }

    public void Refresh() {
        var currentLevel = PlayerPrefs.GetInt("currentLevel", 1);
        
        for (var i = 0; i < _totalLevels; i++) {
            _buttons[i].interactable = i <  currentLevel;
        }
    }
}