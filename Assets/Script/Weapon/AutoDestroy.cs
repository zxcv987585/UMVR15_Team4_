using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDestroy : MonoBehaviour
{
    public float DestroyTime = 1.2f;
    private void Start()
    {
        Destroy(gameObject, DestroyTime);
    }
}
