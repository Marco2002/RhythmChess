using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    VisualElement root;
    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;
    }

    public void setLevel(string level) {
        root.Q<Label>("LabelLevel").text = level;
    }
}
