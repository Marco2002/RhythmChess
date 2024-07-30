using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

public class MenuUI : MonoBehaviour {
    private VisualElement root;
    private Button backButton;
    private void Start() {
        root = GetComponent<UIDocument>().rootVisualElement;

        backButton = root.Q<Button>("BackButton");
        if (backButton == null) {
            Debug.LogError("Button with the specified name not found.");
            return;
        }

        backButton.clicked += OnButtonClick;
    }

    private void OnButtonClick() {
        SceneManager.LoadScene("Game");
    }

}
