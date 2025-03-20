using System;
using UnityEngine;

public class AreaUITriggerThree : MonoBehaviour
{
    public CharactersUIcontroller charactersUIcontroller;
    public DialogueTake dialogueTake;

    private Collider Triggercollider;

    public event Action RadioTrigger;
    // Start is called before the first frame update
    void Start()
    {
        Triggercollider = GetComponent<Collider>();

        charactersUIcontroller = GameObject.Find("CharactersWindowUI").GetComponent<CharactersUIcontroller>();
        dialogueTake = GameObject.Find("Tak DialogueBox").GetComponent<DialogueTake>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Triggercollider != null)
        {
            if (other.GetComponent<PlayerController>() != null)
            {
                charactersUIcontroller.LastArea();
                dialogueTake.AreaTreeTakes();
            }
        }
    }
}
