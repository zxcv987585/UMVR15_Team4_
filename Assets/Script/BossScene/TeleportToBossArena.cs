using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

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

    private IEnumerator TeleportSequence()
    {
        //1.=====啟動傳送特效=====
        vfxHyperDriveEffect();
        magicCircleEffect();
        yield return new WaitForSeconds(200000000f);  // 等待 1 秒

        //2.=====特效加強 + 黑屏 (未來會加)=====

        //3.=====傳送=====
        Debug.Log("進行傳送...");
        if (player != null)
        {
            CharacterController controller = player.GetComponent<CharacterController>();
            if (controller != null)
            {
                controller.enabled = false;
                player.transform.position = targetPos.transform.position;
                player.transform.rotation = Quaternion.Euler(0.8f, 48f, 0f);

                controller.enabled = true;
            }
        }
        if (mainCamera != null)
        {
            Vector3 playerEuler = player.transform.eulerAngles;
            mainCamera.SetCameraRotation(playerEuler.y, playerEuler.x);
        }
        yield return new WaitForSeconds(0.5f);  // 等 0.5 秒讓攝影機適應

        //4.=====傳送後演出=====
        Debug.Log("傳送完成！");
    }

    void Update()
    {
        if (isUseable == true && Input.GetKeyDown(KeyCode.P))
        {
            StartCoroutine(TeleportSequence());
        }
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
        StartCoroutine(ChangeVector3(new Vector3(1.15f, 1.15f, 1.15f), new Vector3(1.5f, 1.5f, 1.5f), 0.2f,
            value => magicCircle.gameObject.transform.localScale = value));

        var magicCircleRotationOverLifetime = magicCircle.rotationOverLifetime;
        magicCircleRotationOverLifetime.z = new ParticleSystem.MinMaxCurve(45f * Mathf.Deg2Rad);

        var magicCircleSideRotationOverLifetime = magicCircleSide.rotationOverLifetime;
        magicCircleSideRotationOverLifetime.y = new ParticleSystem.MinMaxCurve(90f * Mathf.Deg2Rad);
    }
}
