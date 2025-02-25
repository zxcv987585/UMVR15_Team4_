using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookCamera : MonoBehaviour
{
    private void Update()
    {
        transform.forward = Camera.main.transform.forward;
    }
}
