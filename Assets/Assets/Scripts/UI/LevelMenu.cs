using UnityEngine;
using UnityEngine.UI;

public class LevelMenu : MonoBehaviour {
    [SerializeField] private GameObject _levelButtonPrefab;
    [SerializeField] private Transform _buttonContainer;
    [SerializeField] private int _totalLevels = 10;
    [SerializeField] public Controller _controller;

    private void Start() {
        for (int i = 1; i <= _totalLevels; i++) {
            int levelIndex = i;
            GameObject button = Instantiate(_levelButtonPrefab, _buttonContainer);
            button.GetComponentInChildren<Text>().text = "Level " + levelIndex;
            button.GetComponent<Button>().onClick.AddListener(() => LoadLevel(levelIndex));
        }
    }

    private void LoadLevel(int levelIndex) {
        _controller.LoadLevel(levelIndex);
        gameObject.SetActive(false);
    }
}