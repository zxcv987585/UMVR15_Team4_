using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonUISound : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler
{
    [SerializeField] private string _selectSound;
    [SerializeField] private string _clickSound;

    private Button _button;

    private void Start()
    {
        _button = GetComponent<Button>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound("ButtonClick", transform.position);
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        AudioManager.Instance.PlaySound("ButtonSelect", transform.position);
    }
}
