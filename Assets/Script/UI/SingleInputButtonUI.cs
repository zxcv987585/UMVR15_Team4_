using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SingleInputButtonUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _bindText;
    [SerializeField] private Button _bindButton;
    [SerializeField] private List<TextMeshProUGUI> _bindButtonText;
    [SerializeField] private GameInput.Bind _bind;

    private void Start()
    {
        SetBind(_bind);
    }

    public void SetBind(GameInput.Bind bind)
    {
        _bind = bind;
        
        _bindText.text = GameInput.Instance.GetBindChinese(_bind);
        UpdateVisualText();
        
        _bindButton.onClick.AddListener(() => 
        {
            _bindButtonText.ForEach(bindButtonText => bindButtonText.text = "...");
            GameInput.Instance.ReBindKeyboard(bind, UpdateVisualText, ShowFailBindMessage);
        });
    }
    
    private void UpdateVisualText()
    {
        _bindButtonText.ForEach(bindButtonText => bindButtonText.text = GameInput.Instance.GetBindText(_bind));
    }
    
    private void ShowFailBindMessage()
    {
        UpdateVisualText();
        this.PlaySound("BindFail");
        //Debug.Log("按鍵衝突");
    }
}
