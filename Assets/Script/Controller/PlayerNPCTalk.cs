using UnityEngine;

public class PlayerNPCTalk : MonoBehaviour
{
    public LayerMask NPCLayer;
    public BossSceneDialogue DialogueUI;
    private float interactRange = 1.5f;

    // Start is called before the first frame update
    private void Awake()
    {
        DialogueUI = GameObject.Find("BossSceneDialogueBox").GetComponent<BossSceneDialogue>();
    }
    void Start()
    {
        NPCLayer = LayerMask.GetMask("NPC");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.F))
        {
            if (DialogueUI.IsTalk) return;

            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange, NPCLayer);
            if (colliderArray == null) return;

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out NPCInteractable interactable))
                {
                    interactable.Interact();
                    break;
                }
            }
        }

    }
}
