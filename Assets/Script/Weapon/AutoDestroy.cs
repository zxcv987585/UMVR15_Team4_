using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    private void Start()
    {
        GameObjectDistroy();
    }

    void  GameObjectDistroy()
    {
        Destroy(gameObject, 1.2f);
    }
}
