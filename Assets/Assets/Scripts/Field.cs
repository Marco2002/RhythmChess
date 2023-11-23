using Unity.VisualScripting;
using UnityEngine;

public class Field : MonoBehaviour {
    
    public void Init(bool isOffset, bool isFlagRegion) {
        Renderer renderer = gameObject.GetComponent<Renderer>();
        renderer.material.shader = Shader.Find("Standard");
        if (isFlagRegion) {
            renderer.material.SetColor("_Color", isOffset ? ColorScheme.fieldFlagOffset : ColorScheme.fieldFlag);
        } else {
            renderer.material.SetColor("_Color", isOffset ? ColorScheme.fieldOffset : ColorScheme.field);
        }
    }
}
