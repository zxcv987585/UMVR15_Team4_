using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportToBossArena : MonoBehaviour
{
    public Collider portal;
    public bool isUseable = false;
    public Transform targetPos;
    public Transform player;
    public CameraController mainCamera;

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
        Debug.Log("開始傳送特效！");

        //1.=====啟動傳送特效=====
        /*portalEffect.Play();*/  // 假設 portalEffect 是你的傳送門粒子
        yield return new WaitForSeconds(1f);  // 等待 1 秒

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
        //if (isUseable == true && Input.GetKeyDown(KeyCode.P)) 
        //{
        //    if (player != null)
        //    {
        //        CharacterController controller = player.GetComponent<CharacterController>();
        //        if (controller != null)
        //        {
        //            controller.enabled = false;
        //            player.transform.position = targetPos.transform.position;
        //            player.transform.rotation = Quaternion.Euler(0.8f, 48f,0f);
                    
        //            controller.enabled = true;
        //        }
        //    }
        //    if (mainCamera != null)
        //    {
        //        Vector3 playerEuler = player.transform.eulerAngles;
        //        mainCamera.SetCameraRotation(playerEuler.y, playerEuler.x);
        //    }
        //}
    }
}
