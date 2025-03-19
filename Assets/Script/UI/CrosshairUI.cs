using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;

public class CrosshairUI : MonoBehaviour
{
    [SerializeField] private PostProcessVolume postProcessVolume;
    private Vignette vignette;

    [SerializeField] private RawImage crosshairImage;
    private PlayerController player;
    private EasyInOut easyInOut;

    private Coroutine crosshairCoroutine; //記錄當下的Coroutine
    private Coroutine frameCoroutine;     //記錄當下的Coroutine

    private void Start()
    {
        player = FindObjectOfType<PlayerController>();
        if (player != null)
        {
            player.aimState.OnAim += ToggleCrosshair;
        }
        crosshairImage.gameObject.SetActive(false);

        if (postProcessVolume.profile.TryGetSettings(out vignette))
        {
            vignette.intensity.value = 0f;
            postProcessVolume.isGlobal = false;
        }
    }

    private void ToggleCrosshair(bool isAiming)
    {
        if (isAiming)
        {
            crosshairImage.gameObject.SetActive(true);
            postProcessVolume.isGlobal = true;

            // 停止舊的動畫，確保每次進入瞄準時都會重新播放
            if (crosshairCoroutine != null) StopCoroutine(crosshairCoroutine);
            if (frameCoroutine != null) StopCoroutine(frameCoroutine);

            // 啟動新的動畫
            crosshairCoroutine = StartCoroutine(CrosshairAnimation());
            frameCoroutine = StartCoroutine(FrameAnimationIn());
        }
        else
        {
            // 瞄準結束時，停止動畫並隱藏 UI
            if (crosshairCoroutine != null) StopCoroutine(crosshairCoroutine);
            if (frameCoroutine != null) StopCoroutine(frameCoroutine);

            // 立即隱藏 Crosshair
            crosshairImage.gameObject.SetActive(false);

            // 開始淡出 Vignette
            frameCoroutine = StartCoroutine(FrameAnimationOut());
        }
    }

    private IEnumerator CrosshairAnimation()
    {
        easyInOut = FindObjectOfType<EasyInOut>();

        RectTransform crosshairRect = crosshairImage.GetComponent<RectTransform>();
        crosshairRect.rotation = Quaternion.Euler(0, 0, 45f);
        crosshairRect.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        // 旋轉動畫
        yield return StartCoroutine(easyInOut.ChangeValue(
            45f, 0f, 0.25f,
            value => crosshairRect.rotation = Quaternion.Euler(0, 0, value),
            EasyInOut.EaseOut));

        // 縮放動畫
        yield return StartCoroutine(easyInOut.ChangeValue(
            new Vector3(1.5f, 1.5f, 1.5f), Vector3.one, 0.25f,
            value => crosshairRect.localScale = value,
            EasyInOut.EaseOut));
    }

    private IEnumerator FrameAnimationIn()
    {
        float startValue = vignette.intensity.value;
        float endValue = 0.2f;
        float duration = 0.35f;
        float timer = 0f;

        postProcessVolume.isGlobal = true;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            vignette.intensity.value = Mathf.Lerp(startValue, endValue, timer / duration);
            yield return null;
        }

        vignette.intensity.value = endValue;
    }

    private IEnumerator FrameAnimationOut()
    {
        float startValue = vignette.intensity.value;
        float endValue = 0f;
        float duration = 0.35f;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.unscaledDeltaTime;
            vignette.intensity.value = Mathf.Lerp(startValue, endValue, timer / duration);
            yield return null;
        }

        vignette.intensity.value = endValue;
        postProcessVolume.isGlobal = false;  
    }

    //開槍動畫
    public void CrosshairShootAnimation()
    {
        if (crosshairCoroutine != null) StopCoroutine(crosshairCoroutine);
        crosshairCoroutine = StartCoroutine(ShootEffect());
    }

    private IEnumerator ShootEffect()
    {
        RectTransform crosshairRect = crosshairImage.GetComponent<RectTransform>();

        yield return StartCoroutine(easyInOut.ChangeValue(
            new Vector3(1.3f, 1.3f, 1.3f), Vector3.one, 0.05f,
            value => crosshairRect.localScale = value,
            EasyInOut.EaseOut));
    }
   
    private void OnDestroy()
    {
        if (player != null)
        {
            player.aimState.OnAim -= ToggleCrosshair;
        }
    }
}
