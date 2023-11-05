using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Field : MonoBehaviour {
    [SerializeField] private Color _white, _black, _disabled, _flag;
    [SerializeField] private SpriteRenderer _renderer;

    public void Init(bool isOffset, bool isDisabled, bool isFlagRegion) {
        if(isDisabled) {
            _renderer.color = _disabled;
        } else if(isFlagRegion) {
            _renderer.color = _flag;
        } else { 
            _renderer.color = isOffset ? _black : _white;
        }
    }
}
