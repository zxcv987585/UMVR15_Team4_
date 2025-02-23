using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPBarUI : MonoBehaviour
{
    [SerializeField] Image hpImage;

    public void SetHP(float hpSituation)
    {
        hpImage.fillAmount = hpSituation;
    }

    private void LateUpdate()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
