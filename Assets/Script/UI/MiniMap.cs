using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
