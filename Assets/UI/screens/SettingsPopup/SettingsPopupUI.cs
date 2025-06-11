using System;
using UnityEngine;
using UnityEngine.UIElements;

public class SettingsPopupUI : MonoBehaviour {
    [SerializeField] private UIDocument _uiDocument;
    private Popup _popup;
    private SettingsControl _soundControl, _vibrationControl, _countInBeatsControl;
    private VisualElement _root;
    public event Action OnCloseButtonClicked, OnTutorialClicked, OnPrivacyPolicyClicked;
    public event Action<bool> OnSoundSettingsChanged, OnVibrationSettingsChanged;
    public event Action<int> OnCountInBeatsSettingsChanged;

    private void OnEnable() {
        _root = _uiDocument.rootVisualElement;
        _popup = _root.Q<Popup>("SettingsPopup");
        var closeButton = _root.Q<Button>("ButtonClose");
        _soundControl = _root.Q<SettingsControl>("SettingsControlSound");
        _vibrationControl = _root.Q<SettingsControl>("SettingsControlVibration");
        _countInBeatsControl = _root.Q<SettingsControl>("SettingsControlCountInBeats");
        var howToPlayControl = _root.Q<SettingsControl>("SettingsControlHowToPlay");
        var privacyPolicyControl = _root.Q<SettingsControl>("SettingsControlPrivacyPolicy");
        
        closeButton.clicked += () => {
            Close();
            OnCloseButtonClicked?.Invoke();
        };
        
        _popup.OnTappedOutside += () => {
            Close();
            OnCloseButtonClicked?.Invoke();
        };
        
        _soundControl.OnValueChanged += (value) => {
            PlayerPrefs.SetInt("soundEnabled", value ? 1 : 0);
            PlayerPrefs.Save();
            OnSoundSettingsChanged?.Invoke(value);
        };
        
        _vibrationControl.OnValueChanged += (value) => {
            PlayerPrefs.SetInt("vibrationEnabled", value ? 1 : 0);
            PlayerPrefs.Save();
            OnVibrationSettingsChanged?.Invoke(value);
        };
        
        _countInBeatsControl.OnValueChanged += (value) => {
            PlayerPrefs.SetInt("countInBeats", value ? 4 : 2);
            PlayerPrefs.Save();
            OnCountInBeatsSettingsChanged?.Invoke(value ? 4 : 2);
        };
        
        howToPlayControl.OnButtonClicked += () => OnTutorialClicked?.Invoke();
        privacyPolicyControl.OnButtonClicked += () => OnPrivacyPolicyClicked?.Invoke();
    }

    public void Init() {
        _vibrationControl.Value = PlayerPrefs.GetInt("vibrationEnabled") == 1;
        _countInBeatsControl.Value = PlayerPrefs.GetInt("countInBeats") == 4;
        _soundControl.Value = PlayerPrefs.GetInt("soundEnabled") == 1;
    }

    public void Open() {
        _popup.Open();
        _root.style.visibility = Visibility.Visible;
    }

    public void Close() {
        _popup.Close();
        _root.style.visibility = Visibility.Hidden;
    }
}