using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapFadeIn : MonoBehaviour
{
    [SerializeField] private CanvasGroup MiniMapCanvasGroup;
    [SerializeField] private float FadeDuration = 3f;

    // Start is called before the first frame update
    void Start()
    {
        MiniMapCanvasGroup.alpha = 0f;
        StartCoroutine(FadeIn());
    }

    private IEnumerator FadeIn()
    {
        yield return new WaitForSeconds(1.6f);
        float elapse = 0f;
        while(elapse < FadeDuration)
        {
            MiniMapCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapse / FadeDuration);
            elapse += Time.deltaTime;
            yield return null;
        }
        MiniMapCanvasGroup.alpha = 1f;
    }
}
