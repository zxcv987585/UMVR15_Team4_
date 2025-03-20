using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TeleportToLoadScene : MonoBehaviour
{
    public Collider portal;
    public bool isUseable = false;
    public bool isBeUse = false;
    public Transform player;
    public CameraController mainCamera;

    public ParticleSystem vfxHyperDrive;
    public ParticleSystem magicCircle;
    public ParticleSystem magicCircleSide;
    public ParticleSystem magicCircleSparks;
    public ParticleSystem vfxImplosion;
    public Image whiteScreen;
    public Image blackScreen;

    public RectTransform isUseableUI;

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

    private void Start()
    {
        GameInput.Instance.OnInteraction += Teleport;
        
        player = GameObject.FindGameObjectWithTag("Player").transform;
        mainCamera = FindAnyObjectByType<CameraController>();
    }

    private void Teleport()
    {
        if (isUseable == true && isBeUse == false)
        {
            AudioManager.Instance.PlaySound("Teleport", transform.position, false, 4f);

            StartCoroutine(TeleportSequence());
            isUseable = false;
            isBeUse = true;
        }
    }

    private IEnumerator TeleportSequence()
    {
        StartCoroutine(isUseableUIoff());

        var playerPos = player.transform.position;
        CharacterController charController = player.GetComponent<CharacterController>();
        PlayerController playerController = player.GetComponent<PlayerController>();
        AnimatorController animatorController = player.GetComponent<AnimatorController>();
        PlayerStateMachine playerStateMachine = player.GetComponent<PlayerStateMachine>();
        Animator animator = player.GetComponent<Animator>();
        if (charController != null) charController.enabled = false;
        if (playerController != null) playerController.enabled = false;
        if (animatorController != null) animatorController.enabled = false;
        if (playerStateMachine != null) playerStateMachine.enabled = false;
        animator.SetBool("Run", false);
        animator.SetBool("Sprint", false);
        
        EasyInOut easyInOut = FindObjectOfType<EasyInOut>();

        vfxHyperDriveEffect();
        magicCircleEffect();
        whiteScreen.gameObject.SetActive(true);
        animator.Play("Idle");
        yield return new WaitForSeconds(1f);

        vfxImplosion.transform.position = new Vector3(playerPos.x, playerPos.y + 1, playerPos.z);
        vfxImplosion.gameObject.SetActive(true);
        yield return new WaitForSeconds(1.25f);
        StartCoroutine(easyInOut.ChangeValue(
            new Vector4(1f, 1f, 1f, 0f),
            new Vector4(1f, 1f, 1f, 1f),
            0.75f,
            value => whiteScreen.color = value,
            EasyInOut.EaseOut
            ));
        yield return new WaitForSeconds(0.75f);

        vfxHyperDrive.gameObject.SetActive(false);
        magicCircle.gameObject.SetActive(false);
        magicCircleSide.gameObject.SetActive(false);
        magicCircleSparks.gameObject.SetActive(false);
        vfxImplosion.gameObject.SetActive(false);
        charController.enabled = true;
        playerController.enabled = true;
        animatorController.enabled = true;
        playerStateMachine.enabled = true;

        yield return new WaitForSeconds(0.25f);
        blackScreen.gameObject.SetActive(true);
        StartCoroutine(easyInOut.ChangeValue(
            new Vector4(0f, 0f, 0f, 0f),
            new Vector4(0f, 0f, 0f, 1f),
            0.75f,
            value => blackScreen.color = value,
            EasyInOut.EaseOut
            ));
        yield return new WaitForSeconds(0.75f);


        whiteScreen.gameObject.SetActive(false);

        LoadManager.Load(LoadManager.Scene.AN_Demo_Boss);
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

    //=====Change Coroutine=====
    private IEnumerator ChangeFloat(float start, float end, float duration, System.Action<float> onUpdate)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float value = Mathf.Lerp(start, end, timer / duration);
            onUpdate(value);
            yield return null;
        }
        onUpdate(end);
    }

    private IEnumerator ChangeVector4(Vector4 start, Vector4 end, float duration, System.Action<Vector4> onUpdate)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            Vector4 value = Vector4.Lerp(start, end, timer / duration);
            onUpdate(value);
            yield return null;
        }
        onUpdate(end);
    }

    private IEnumerator ChangeVector3(Vector3 start, Vector3 end, float duration, System.Action<Vector3> onUpdate)
    {
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            Vector3 value = Vector3.Lerp(start, end, timer / duration);
            onUpdate(value);
            yield return null;
        }
        onUpdate(end);
    }

    //=====vfxHyperDrive effect changer=====
    private void vfxHyperDriveEffect()
    {
        var vfxHyperDriveMain = vfxHyperDrive.main;
        vfxHyperDriveMain.startColor = new Color(162 / 255f, 215 / 255f, 255 / 255f, 255 / 255f);
        vfxHyperDriveMain.startSpeed = new ParticleSystem.MinMaxCurve(12f, 25f);
        StartCoroutine(ChangeFloat(0.3f, 0.7f, 3f, value => vfxHyperDriveMain.simulationSpeed = value));

        var vfxHyperDriveEmission = vfxHyperDrive.emission;
        StartCoroutine(ChangeFloat(10f, 30f, 3f, value => vfxHyperDriveEmission.rateOverTime = value));

        var vfxHyperDriveShape = vfxHyperDrive.shape;
        vfxHyperDriveShape.scale = new Vector3(0.65f, 0.65f, 0.65f);
    }

    //=====vfxHyperDrive effect changer=====
    private void magicCircleEffect()
    {
        EasyInOut easyInOut = FindObjectOfType<EasyInOut>();

        if (easyInOut == null) return;

        StartCoroutine(easyInOut.ChangeValue(
            new Vector3(1.15f, 1.15f, 1.15f),
            new Vector3(1.30f, 1.30f, 1.30f),
            1f,
            value => magicCircle.gameObject.transform.localScale = value,
            EasyInOut.EaseOut
            ));

        var magicCircleRotationOverLifetime = magicCircle.rotationOverLifetime;
        magicCircleRotationOverLifetime.z = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);

        var magicCircleSideRotationOverLifetime = magicCircleSide.rotationOverLifetime;
        magicCircleSideRotationOverLifetime.y = new ParticleSystem.MinMaxCurve(90f * Mathf.Deg2Rad);
        magicCircleSparks.gameObject.SetActive(true);
    }
}
