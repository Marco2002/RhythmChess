using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour {
    [SerializeField] private SpriteRenderer _renderer;

    public void Init(bool isOffset, bool isDisabled, bool isFlagRegion) {
        if(isDisabled) {
            _renderer.color = ColorScheme.fieldDisabled;
        } else if(isFlagRegion) {
            _renderer.color = isOffset ? ColorScheme.fieldFlagOffset : ColorScheme.fieldFlag;
        } else { 
            _renderer.color = isOffset ? ColorScheme.fieldOffset : ColorScheme.field;
        }
    }
}
