using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Transform Player;

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    // Update is called once per frame
    void LateUpdate()
    {
        Vector3 newPosition = Player.position;
        newPosition.y = transform.position.y;
        transform.position = newPosition;
    }
}
