using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaUITraggerTwo : MonoBehaviour
{
    public TeachingUIController teachingUI;

    private Collider Triggercollider;

    public event Action RadioTrigger;
    // Start is called before the first frame update
    void Start()
    {
        Triggercollider = GetComponent<Collider>();

        teachingUI = GameObject.Find("Teaching DialogueBox").GetComponent<TeachingUIController>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Triggercollider != null)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                teachingUI.CloseWindows();
            }
        }
    }
}
