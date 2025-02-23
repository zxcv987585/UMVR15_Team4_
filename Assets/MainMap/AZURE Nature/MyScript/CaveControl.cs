using System.Collections;
using UnityEngine;

public class CaveControl : MonoBehaviour
{
    public Color targetFogColor = new Color();
    private Color defaultFogColor;

    public Light caveLight;
    public float caveLightValue;
    private float defaultLightValue;

    public float changeSpeed = 1.2f;
    private bool isFading = false;

    private void Start()
    {
        defaultFogColor = RenderSettings.fogColor;
        defaultLightValue = caveLight.intensity;
        caveLight.intensity = 0f;
    }

    private IEnumerator FadingChange(bool enterCave)
    {
        isFading = true;
        float timer = 0f;

        Color startFogColor = RenderSettings.fogColor;
        Color endFogColor = enterCave ? targetFogColor : defaultFogColor;

        float startLight = caveLight.intensity;
        float endLight = enterCave ? caveLightValue : defaultLightValue;

        while (timer < changeSpeed)
        {
            timer += Time.deltaTime;
            float t = timer / changeSpeed; // 讓 t 在 0~1 之間平滑變化

            RenderSettings.fogColor = Color.Lerp(startFogColor, endFogColor, t);
            caveLight.intensity = Mathf.Lerp(startLight, endLight, t);

            yield return null; // **等待一個 frame**
        }

        RenderSettings.fogColor = endFogColor;
        caveLight.intensity = endLight;
        isFading = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isFading == false)
        {
            StartCoroutine(FadingChange(true));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isFading == false)
        {
            StartCoroutine(FadingChange(false));
        }
    }
}