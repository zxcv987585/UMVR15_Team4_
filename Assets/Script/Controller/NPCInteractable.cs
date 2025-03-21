using System;
using System.Collections;
using UnityEngine;

public class NPCInteractable : MonoBehaviour
{
    public BossSceneDialogue TalkUI;

    private void Start()
    {
        TalkUI = GameObject.Find("TalkUI").GetComponent<BossSceneDialogue>();
    }

    public void Interact()
    {
        TalkUI.FirstTalkToPlayer();
    }
}
