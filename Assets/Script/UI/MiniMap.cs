using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Transform Player;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (FindObjectsOfType<MiniMap>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
    }
    private void Start()
    {
        Player = GameObject.FindGameObjectWithTag("Player").transform;
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPosition = Player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }

    //訂閱跳轉場景事件
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            Destroy(gameObject);
            OnDestroy();
        }
    }
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
