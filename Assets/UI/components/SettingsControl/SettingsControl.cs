using System;
using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class SettingsControl: VisualElement {
    private Label label, icon;
    private Toggle checkbox;
    private RadioButtonGroup radioGroup;
    private Button button;
    public event Action OnButtonClicked;
    public event Action<bool> OnValueChanged;
    public const int TYPE_CHECKBOX = 0, TYPE_RADIO = 1, TYPE_BUTTON = 2;
    
    [UxmlAttribute]
    public int Type {
        get => checkbox.style.visibility == Visibility.Visible ? TYPE_CHECKBOX : 
            button.style.visibility == Visibility.Visible ? TYPE_BUTTON : TYPE_RADIO;
        set {
            checkbox.style.visibility = value == TYPE_CHECKBOX ? Visibility.Visible : Visibility.Hidden;
            button.style.visibility = value == TYPE_BUTTON ? Visibility.Visible : Visibility.Hidden;
            radioGroup.style.visibility = value == TYPE_RADIO ? Visibility.Visible : Visibility.Hidden;
        }
    }
    
    [UxmlAttribute]
    public string LabelText {
        get => label.text;
        set => label.text = value;
    }
    
    [UxmlAttribute]
    public bool Value {
        get => checkbox.value;
        set {
            checkbox.value = value;
            radioGroup.value = value ? 1 : 0;
        }
    }

    [UxmlAttribute]
    public string IconGlyph {
        get => icon.text;
        set => icon.text = value;
    }
    
    [UxmlAttribute]
    public bool UseVibrationImage {
        get => icon.ClassListContains("settings-control-icon_image");
        set {
            if (value) {
                icon.AddToClassList("settings-control-icon_image");
            } else {
                icon.RemoveFromClassList("settings-control-icon_image");
            }
        }
    }

    public SettingsControl() {
        var visualTree = Resources.Load<VisualTreeAsset>("UI/settingsControl");
        visualTree.CloneTree(this);
        
        label = this.Q<Label>("Label");
        checkbox = this.Q<Toggle>("Checkbox");
        radioGroup = this.Q<RadioButtonGroup>("RadioButtonGroup");
        button = this.Q<Button>("Button");
        icon = this.Q<Label>("Icon");

        button.clicked += () => OnButtonClicked?.Invoke();
        checkbox.RegisterValueChangedCallback((changeEvent) => {
            OnValueChanged?.Invoke(changeEvent.newValue);
        });
        radioGroup.RegisterValueChangedCallback((changeEvent) => {
            OnValueChanged?.Invoke(changeEvent.newValue > 0);
        });
    }
}
