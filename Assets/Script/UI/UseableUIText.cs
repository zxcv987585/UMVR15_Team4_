using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UseableUIText : MonoBehaviour
{
    private TextMeshProUGUI _useText;

    private void OnEnable()
    {
        if(GameInput.Instance == null) return;

        if(_useText == null) _useText = GetComponent<TextMeshProUGUI>();

        _useText.text = string.Format("按 {0} 啟動", GameInput.Instance.GetBindText(GameInput.Bind.Interaction));

    }
}
