using UnityEngine;
using UnityEngine.SceneManagement;

public class SpawnPointFinder : MonoBehaviour
{
    [SerializeField] private Transform player;
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
        if(scene.name == "AN_Demo_Boss")
        {
            spawnPoint = GameObject.Find("PlayerSpawnPoint");
            player.SetLocalPositionAndRotation(spawnPoint.transform.position, player.transform.rotation);
            Physics.SyncTransforms();
        }
    }
}
