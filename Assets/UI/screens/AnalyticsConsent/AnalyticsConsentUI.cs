using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class AnalyticsConsentUI : MonoBehaviour {
    public System.Action OnConsentAccepted;
    public System.Action OnConsentDeclined;
    
    [SerializeField] private TMP_Text _text;
    [SerializeField] private Button _allowButton;
    [SerializeField] private Button _declineButton;

    private void Update() {
        // Check for clicks on a link
        if (!Input.GetMouseButtonDown(0)) return;
        var linkIndex = TMP_TextUtilities.FindIntersectingLink(_text, Input.mousePosition, null);
        if (linkIndex == -1) return;
        var linkInfo = _text.textInfo.linkInfo[linkIndex];
        var url = linkInfo.GetLinkID();
        Application.OpenURL(url);
    }
    
    public void Init() {
        _allowButton.onClick.AddListener(() => OnConsentAccepted?.Invoke());
        _declineButton.onClick.AddListener(() => OnConsentDeclined?.Invoke());
        _text.richText = true;
    }
}