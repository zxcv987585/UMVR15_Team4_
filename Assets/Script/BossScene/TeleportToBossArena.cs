using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TeleportToBossArena : MonoBehaviour
{
    public Collider portal;
    public bool isUseable = false;
    public Transform targetPos;
    public Transform player;
    public CameraController mainCamera;

    public ParticleSystem vfxHyperDrive;
    public ParticleSystem magicCircle;
    public ParticleSystem magicCircleSide;
    public ParticleSystem magicCircleSparks;
    public ParticleSystem vfxImplosion;
    public Image whiteScreen;
    public Image blackScreen;

    private void Awake()
    {
        if(whiteScreen == null || blackScreen == null)
        {
            var BattleUI = GameObject.Find("BattleUICanvas");
            
            if(BattleUI != null)
            {
                whiteScreen = BattleUI.transform.Find("whiteScreen")?.GetComponent<Image>();
                blackScreen = BattleUI.transform.Find("blackScreen")?.GetComponent<Image>();
            }
        }
        if (player == null)
        {
            GameObject playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null)
            {
                player = playerObj.transform;
            }
        }

        if (mainCamera == null)
        {
            GameObject cameraObj = GameObject.FindWithTag("MainCamera");
            if (cameraObj != null)
            {
                mainCamera = cameraObj.GetComponent<CameraController>();
            }
        }
    }

    private void Start()
    {
        EasyInOut easyInOut = FindObjectOfType<EasyInOut>();
        StartCoroutine(easyInOut.ChangeValue(
            new Vector4(0f, 0f, 0f, 1f),
            new Vector4(0f, 0f, 0f, 0f),
            1.5f,
            value => blackScreen.color = value,
            EasyInOut.EaseIn
            ));

        GameInput.Instance.OnInteraction += Teleport;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && isUseable == false)
        {
            isUseable = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && isUseable == true)
        {
            isUseable = false;
        }
    }

    private void Teleport()
    {
        if (isUseable == true)
        {
            AudioManager.Instance.PlaySound("Teleport", transform.position, false, 4f);

            StartCoroutine(TeleportSequence());
            isUseable = false;
        }
    }

    private IEnumerator TeleportSequence()
    {
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

        //1.=====啟動傳送門特效=====
        vfxHyperDriveEffect();
        magicCircleEffect();
        whiteScreen.color = new Vector4(1f, 1f, 1f, 0f);
        whiteScreen.gameObject.SetActive(true);
        animator.Play("Idle");
        yield return new WaitForSeconds(1f);

        //2.=====特效加強=====
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

        //3.=====傳送=====
        if (player != null)
        {
            player.transform.position = targetPos.transform.position;
            player.transform.rotation = Quaternion.Euler(0.8f, 48f, 0f);
            
        }
        if (mainCamera != null)
        {
            Vector3 playerEuler = player.transform.eulerAngles;
            mainCamera.SetCameraRotation(playerEuler.y, playerEuler.x);
        }
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(ChangeVector4(Color.white, new Vector4(1f, 1f, 1f, 0f), 2f, value => whiteScreen.color = value));
        yield return new WaitForSeconds(0.5f);

        //4.=====ｶﾇｰeｫ蘯t･X=====
        charController.enabled = true;
        playerController.enabled = true;
        animatorController.enabled = true;
        playerStateMachine.enabled = true;
        vfxHyperDrive.gameObject.SetActive(false);
        magicCircle.gameObject.SetActive(false);
        magicCircleSide.gameObject.SetActive(false);
        magicCircleSparks.gameObject.SetActive(false);
        vfxImplosion.gameObject.SetActive(false);

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
            new Vector3(1.5f, 1.5f, 1.5f), 
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
