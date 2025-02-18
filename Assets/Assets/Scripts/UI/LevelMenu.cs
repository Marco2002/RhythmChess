using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour {
    [SerializeField] private Button _levelButtonPrefab;
    [SerializeField] private Transform _buttonContainer;
    [SerializeField] public Controller _controller;

    public const int UNBEATEN = 0;
    public const int ONE_STAR = 1;
    public const int TWO_STAR = 2;
    public const int THREE_STAR = 3;
    private const string STAR_CHARACTER = "\uf005";
    
    private Button[] _buttons;
    public void Init() {
        // Initialize PlayerPrefs
        if(!PlayerPrefs.HasKey("levelStatus")) {
            var defaultStatus = string.Concat(Enumerable.Repeat(UNBEATEN.ToString(), GameConstants.NUMBER_OF_LEVELS));
            PlayerPrefs.SetString("levelStatus", PlayerPrefs.GetString("levelStatus", defaultStatus));
            PlayerPrefs.Save();
        }
        if(!PlayerPrefs.HasKey("currentLevel")) {
            PlayerPrefs.SetInt("currentLevel", 1);
        }
        
        _buttons = new Button[GameConstants.NUMBER_OF_LEVELS];
        for (var i = 0; i < GameConstants.NUMBER_OF_LEVELS; i++) {
            var levelNumber = i+1;
            var button = Instantiate(_levelButtonPrefab, _buttonContainer);
            _buttons[i] = button;
            button.GetComponentInChildren<Text>().text = levelNumber.ToString();
            button.onClick.AddListener(() => LoadLevel(levelNumber));
        }
        Refresh();
    }

    private void LoadLevel(int levelIndex) {
        _controller.LoadLevel(levelIndex);
        gameObject.SetActive(false);
    }

    public void Refresh() {
        var currentLevel = PlayerPrefs.GetInt("currentLevel");
        var levelStatus = PlayerPrefs.GetString("levelStatus")
            .Select(c => int.Parse(c.ToString())).ToArray();
        for (var i = 0; i < GameConstants.NUMBER_OF_LEVELS; i++) {
            _buttons[i].interactable = i <  currentLevel;
            _buttons[i].GetComponentInChildren<TMP_Text>().text = string.Concat(Enumerable.Repeat(STAR_CHARACTER, levelStatus[i]));
        }
    }
}