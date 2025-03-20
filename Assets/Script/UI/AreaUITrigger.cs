using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaUITrigger : MonoBehaviour
{
    public CharactersUIcontroller charactersUIcontroller;
    public DialogueTake dialogueTake;
    public TeachingUIController teachingUI;

    private Collider Triggercollider;

    public event Action RadioTrigger;
    // Start is called before the first frame update
    void Start()
    {
        Triggercollider = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(Triggercollider != null) 
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                charactersUIcontroller.EnableUI();
                dialogueTake.AreaTwoTakes();
                teachingUI.ChangeText();
            }
        }
    }
}
