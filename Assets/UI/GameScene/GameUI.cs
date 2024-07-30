using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;
public class GameUI : MonoBehaviour {
    private VisualElement root;
    private Button menuButton;
    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;
        
        menuButton = root.Q<Button>("MenuButton");
        if (menuButton == null) {
            Debug.LogError("Button with the specified name not found.");
            return;
        }
        
        menuButton.clicked += OnButtonClick;
    }

    public void SetLevel(string level) {
        root.Q<Label>("HeaderLabel").text = level;
    }
    
    private void OnButtonClick() {
        SceneManager.LoadScene("Menu");
    }
}
