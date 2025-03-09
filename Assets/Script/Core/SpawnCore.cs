using UnityEngine;

public class SpawnCore : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        GameObject spawnPoint = GameObject.Find("PlayerSpawn");
        GameObject camera = GameObject.FindWithTag("MainCamera");
        GameObject CameraspawnPoint = GameObject.Find("CameraSpawn");

        if (player != null && spawnPoint != null)
        {
            player.transform.position = spawnPoint.transform.position;
            player.transform.rotation = spawnPoint.transform.rotation;
        }

        if (camera != null && CameraspawnPoint != null)
        {
            camera.transform.position = CameraspawnPoint.transform.position;
            camera.transform.rotation = CameraspawnPoint.transform.rotation;
        }
    }
}
