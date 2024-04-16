using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UI : MonoBehaviour
{
    private VisualElement root;
    private void OnEnable() {
        root = GetComponent<UIDocument>().rootVisualElement;
    }

    public void SetLevel(string level) {
        root.Q<Label>("LabelLevel").text = level;
    }
}
