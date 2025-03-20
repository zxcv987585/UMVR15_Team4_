using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MySingleTon : MonoBehaviour
{
    private static MySingleTon instance;
    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }
        instance = this;
        DontDestroyOnLoad(gameObject);
    }
}
