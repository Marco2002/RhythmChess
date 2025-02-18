using TMPro;
using UnityEngine;

public class UIController : MonoBehaviour {
    [SerializeField] private LevelMenu _levelMenu;
    [SerializeField] private TMP_Text _levelTitle;

    public void Init(int levelNumber) {
        _levelTitle.text = "Level " + levelNumber;
        _levelMenu.Init();
    }
    
    public void UpdateLevel(int levelNumber) {
        _levelTitle.text = "Level " + levelNumber;
        _levelMenu.Refresh();
    }
}
