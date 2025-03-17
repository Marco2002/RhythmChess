using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour {
    [SerializeField] private LevelMenu _levelMenu;
    [SerializeField] private TMP_Text _levelTitle;
    [SerializeField] private GameObject _fade;
    [SerializeField] private Image _pauseButton;
    [SerializeField] private GameObject _pauseFlash;
    [SerializeField] private Sprite _pauseImage, _resumeImage;

    private TMP_Text pauseFlashText;

    public void Start() {
        pauseFlashText = _pauseFlash.GetComponentInChildren<TMP_Text>();
    }
    
    public void Init(int levelNumber) {
        _levelTitle.text = "LEVEL " + levelNumber;
        _levelMenu.Init();
    }
    
    public void UpdateLevel(int levelNumber) {
        _levelTitle.text = "LEVEL " + levelNumber;
        _levelMenu.Refresh();
    }

    public void Pause() {
        _fade.SetActive(true);
        _pauseButton.sprite = _resumeImage;
    }
    
    public void Resume() {
        _fade.SetActive(false);
        _pauseButton.sprite = _pauseImage;
    }

    public void ShowPauseFlash(bool pause) {
        StartCoroutine(PauseFlashAnimation(pause));
    }
    
    private IEnumerator PauseFlashAnimation(bool pause) {
        if(pause) pauseFlashText.text = "\uf04c";
        else pauseFlashText.text = "\uf04b";
        
        _pauseFlash.SetActive(true);
        const float duration = 0.5f;
        var originalScale = _pauseFlash.transform.localScale;
        var targetScale = originalScale * 1.5f;
        var elapsedTime = 0f;

        while (elapsedTime < duration) {
            _pauseFlash.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsedTime / duration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _pauseFlash.transform.localScale = originalScale;
        _pauseFlash.SetActive(false);
    }
}
