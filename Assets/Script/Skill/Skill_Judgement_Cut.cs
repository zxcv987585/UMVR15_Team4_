using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

public class Skill_Judgement_Cut : BaseSkill
{
    public PostProcessVolume postProcessVolume;
    private ColorGrading colorGrading;
    private Vignette vignette;

    public EasyInOut easyInOut;

    private float radius = 12f;

    private void Awake()
    {
        if (postProcessVolume == null)
        {
            postProcessVolume = FindObjectOfType<PostProcessVolume>();
        }
        if (easyInOut == null)
        {
            easyInOut = FindObjectOfType<EasyInOut>();
        }
    }

    public override void SkillAbility()
    {
        AudioManager.Instance.PlaySound("ElectricSpark", transform.position);
        postProcessVolume.isGlobal = true;
        //�ܷt
        if (postProcessVolume.profile.TryGetSettings(out colorGrading))
        {
            StartCoroutine(easyInOut.ChangeValue(
           new Vector4(255 / 255f, 255 / 255f, 255 / 255f),
           new Vector4(100 / 255f, 100 / 255f, 100 / 255f),
           0.5f,
           value => colorGrading.colorFilter.value = value,
           EasyInOut.EaseOut));
        }
        //�t���ܷt
        if (postProcessVolume.profile.TryGetSettings(out vignette))
        {
            StartCoroutine(easyInOut.ChangeValue(
           0f, 0.4f, 0.5f,
           value => vignette.intensity.value = value,
           EasyInOut.EaseOut));
        }
        StartCoroutine(Spatial_Section(1f));
    }

    private IEnumerator Spatial_Section(float delay)
    {
        yield return new WaitForSeconds(delay);

        transform.position = FindObjectOfType<PlayerController>()?.transform.position ?? Vector3.zero;
        skillParticleSystem.Play();

        //�{�@�U�ե�
        if (postProcessVolume.profile.TryGetSettings(out colorGrading))
        {
            StartCoroutine(easyInOut.ChangeValue(
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f),
            new Vector4(120 / 255f, 120 / 255f, 120 / 255f),
            0.2f,
            value => colorGrading.colorFilter.value = value,
            EasyInOut.EaseOut));

            AudioManager.Instance.PlaySound("MuchSword",transform.position);
            colorGrading.postExposure.overrideState = true;

            StartCoroutine(easyInOut.ChangeValue(
            1.5f, 0f, 1f,
            value => colorGrading.postExposure.value = value,
            EasyInOut.EaseOut));
        }



        Collider[] hitColliderArray = Physics.OverlapSphere(transform.position, radius);
        foreach (Collider hit in hitColliderArray)
        {
            if (hit.TryGetComponent(out IEnemy enemy))
            {
                EnemyManager.Instance.SetEnemyIsPause(2f);
                StartCoroutine(WaitToAttack());
            }
        }

        yield return new WaitForSeconds(2f);
        AudioManager.Instance.PlaySound("Power",transform.position);
        //�A�{�@�U�ե�
        if (postProcessVolume.profile.TryGetSettings(out colorGrading))
        {
            StartCoroutine(easyInOut.ChangeValue(
            new Vector4(120 / 255f, 120 / 255f, 120 / 255f),
            new Vector4(255 / 255f, 255 / 255f, 255 / 255f),
            2f,
            value => colorGrading.colorFilter.value = value,
            EasyInOut.EaseIn));

            StartCoroutine(easyInOut.ChangeValue(
            3.5f, 0f, 1f,
            value => colorGrading.postExposure.value = value,
            EasyInOut.EaseOut));
        }
        //�t���ܦ^
        if (postProcessVolume.profile.TryGetSettings(out vignette))
        {
            StartCoroutine(easyInOut.ChangeValue(
           0.4f, 0f, 2f,
           value => vignette.intensity.value = value,
           EasyInOut.EaseIn));
        }
        yield return new WaitForSeconds(2f);
        colorGrading.postExposure.overrideState = false;
        postProcessVolume.isGlobal = false;
    }
    private IEnumerator WaitToAttack()
    {
        yield return new WaitForSeconds(2f);
        EnemyManager.Instance.TakeAllEnemyDamage(skillDataSO.damage);
    }
}
