using UnityEngine;
using UnityEngine.SceneManagement;

public class CameraSpawnPointFinder : MonoBehaviour
{
    [SerializeField] private Transform Camera;
    private GameObject spawnPoint;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += SceneLoad;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= SceneLoad;
    }

    private void SceneLoad(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "AN_Demo_Boss")
        {
            GameObject player = GameObject.FindGameObjectWithTag("Player");
            if (player != null) 
            {
                CameraController controller = Camera.GetComponent<CameraController>();
                if (controller != null)
                {
                    float playerYaw = player.transform.eulerAngles.y;
                    float desiredPitch = 15f;
                    controller.SetCameraRotation(playerYaw, desiredPitch);
                }
            }
        }
    }
}
