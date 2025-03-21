using UnityEngine;

public class PlayerNPCTalk : MonoBehaviour
{
    public LayerMask NPCLayer;
    private float interactRange = 2f;

    // Start is called before the first frame update
    void Start()
    {
        NPCLayer = LayerMask.GetMask("NPC");
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyUp(KeyCode.N))
        {
            Collider[] colliderArray = Physics.OverlapSphere(transform.position, interactRange, NPCLayer);
            if (colliderArray == null) return;

            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out NPCInteractable interactable))
                {
                    interactable.Interact();
                }
            }
        }

    }
}
