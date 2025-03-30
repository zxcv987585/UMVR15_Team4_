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

    private Coroutine crosshairCoroutine; //�O�����U��Coroutine
    private Coroutine frameCoroutine;     //�O�����U��Coroutine

    private bool IsRightKeyDown = false;

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

    private void Update()
    {
        if (player.IsTeleporting || player.IsCriticalHit)
        {
            ToggleCrosshair(false);

            IsRightKeyDown = Input.GetMouseButton(1);

            if(!player.IsCriticalHit && IsRightKeyDown)
            {
                ToggleCrosshair(true);
            }
        }
    }

    private void ToggleCrosshair(bool isAiming)
    {
        if (isAiming)
        {
            crosshairImage.gameObject.SetActive(true);
            postProcessVolume.isGlobal = true;

            // �����ª��ʵe�A�T�O�C���i�J�˷Ǯɳ��|���s��?E
            if (crosshairCoroutine != null) StopCoroutine(crosshairCoroutine);
            if (frameCoroutine != null) StopCoroutine(frameCoroutine);

            // �Ұʷs���ʵe
            crosshairCoroutine = StartCoroutine(CrosshairAnimation());
            frameCoroutine = StartCoroutine(FrameAnimationIn());
        }
        else
        {
            // �˷ǵ����ɡA�����ʵe������ UI
            if (crosshairCoroutine != null) StopCoroutine(crosshairCoroutine);
            if (frameCoroutine != null) StopCoroutine(frameCoroutine);

            // �ߧY���� Crosshair
            crosshairImage.gameObject.SetActive(false);

            // �}�l�H�X Vignette
            frameCoroutine = StartCoroutine(FrameAnimationOut());
        }
    }

    private IEnumerator CrosshairAnimation()
    {
        easyInOut = FindObjectOfType<EasyInOut>();

        RectTransform crosshairRect = crosshairImage.GetComponent<RectTransform>();
        crosshairRect.rotation = Quaternion.Euler(0, 0, 45f);
        crosshairRect.localScale = new Vector3(1.5f, 1.5f, 1.5f);

        // ����ʵe
        yield return StartCoroutine(easyInOut.ChangeValue(
            45f, 0f, 0.25f,
            value => crosshairRect.rotation = Quaternion.Euler(0, 0, value),
            EasyInOut.EaseOut));

        // �Y��ʵe
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

    //�}�j�ʵe
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
