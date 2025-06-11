using System;
using UnityEngine;
using UnityEngine.UIElements;

public class HowToPlayPopupUI : MonoBehaviour {
    private Popup _popup;
    private SettingsControl _soundControl, _vibrationControl, _countInBeatsControl;
    private VisualElement _root;
    
    public event Action OnCloseButtonClicked;

    public void Init() {
        var uiDocument = GetComponent<UIDocument>();
        _root = uiDocument.rootVisualElement;
        _popup = _root.Q<Popup>("HtpPopup");
        var closeButton = _root.Q<Button>("ButtonClose");
        
        closeButton.clicked += () => {
            Close();
            OnCloseButtonClicked?.Invoke();
        };
    }

    public void Open() {
        _popup.Open();
        Debug.Log("HowToPlayPopup opened");
    }

    public void Close() {
        _popup.Close();
    }
}