using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GetSkillText : MonoBehaviour
{
    public RectTransform flash;
    public RectTransform text;
    public RectTransform group;

    private EasyInOut easyInOut;

    void Awake()
    {
        easyInOut = FindObjectOfType<EasyInOut>();
    }

    void Start()
    {
        StartCoroutine(ShowGetSkillUI());
    }

    private IEnumerator ShowGetSkillUI()
    {
        StartCoroutine(easyInOut.ChangeValue(
           new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 255 / 255f),
           new Vector4(255 / 255f, 255 / 255f, 255 / 255f, 0f),
           2f,
          value => flash.GetComponent<Image>().color = value,
          EasyInOut.EaseOut));

        StartCoroutine(easyInOut.ChangeValue(0f, 12f, 3f,
           value => text.GetComponent<TextMeshProUGUI>().characterSpacing = value,
           EasyInOut.EaseOut));

        yield return new WaitForSeconds(2f);
        StartCoroutine(easyInOut.ChangeValue(1f, 0f, 1f,
           value => group.GetComponent<CanvasGroup>().alpha = value,
           EasyInOut.EaseIn));
    }
}
