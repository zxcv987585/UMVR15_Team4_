using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoverFullHpPp : MonoBehaviour
{
    public bool isUseable = false;

    public bool isBeUse = false; //����u��ϥΤ@��
    public GameObject standbyParticle;
    public ParticleSystem useParticle1;
    public ParticleSystem useParticle2;
    public ParticleSystem useParticle3;
    private PlayerHealth health;

    public RectTransform isUseableUI;

    private void Start()
    {
        isUseableUI = GameObject.Find("isUseableUI").GetComponent<RectTransform>();
        health = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerHealth>();

        GameInput.Instance.OnInteraction += HealthPlayer;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isUseable == false && isBeUse == false)
        {
            isUseable = true;
            StartCoroutine(isUseableUIon());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isUseable == true)
        {
            isUseable = false;
            StartCoroutine(isUseableUIoff());
        }
    }

    private IEnumerator isUseableUIon()
    {
        EasyInOut easyInOut = FindObjectOfType<EasyInOut>();
        
        StartCoroutine(easyInOut.ChangeValue(
            Vector3.one, new Vector3(1.1f, 1.1f, 1.1f), 0.3f,
            value => isUseableUI.localScale = value,
            EasyInOut.EaseOut
            ));

        StartCoroutine(easyInOut.ChangeValue(
            0f, 1f, 0.3f,
            value => isUseableUI.GetComponent<CanvasGroup>().alpha = value,
            EasyInOut.EaseOut
            ));
        yield return null;
    }
    private IEnumerator isUseableUIoff()
    {
        EasyInOut easyInOut = FindObjectOfType<EasyInOut>();

        StartCoroutine(easyInOut.ChangeValue(
            new Vector3(1.1f, 1.1f, 1.1f), Vector3.one, 0.3f,
            value => isUseableUI.localScale = value,
            EasyInOut.EaseIn
            ));

        StartCoroutine(easyInOut.ChangeValue(
            1f, 0f, 0.3f,
            value => isUseableUI.GetComponent<CanvasGroup>().alpha = value,
            EasyInOut.EaseIn
            ));
        yield return null;
    }

    private void HealthPlayer()
    {
        if (isUseable == true && isBeUse == false)
        {
            StartCoroutine(isUseableUIoff());
            isUseable = false;
            isBeUse = true;
            standbyParticle.SetActive(false);

            useParticle1.gameObject.SetActive(true);
            useParticle1.Play();
            useParticle2.gameObject.SetActive(true);
            useParticle2.Play();
            useParticle3.gameObject.SetActive(true);
            useParticle3.Play();
            health.Heal(1000);
            health.HealPP(1000);

            this.PlaySound("Health");
        }
    }
}
