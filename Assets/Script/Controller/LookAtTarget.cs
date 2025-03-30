using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LookAtTarget : MonoBehaviour
{
    private Rig rig;
    private float targetWeight;

    private PlayerController player;
    private BossSceneDialogue DialogueUI;

    private bool IsRightKeyDown = false;

    // Start is called before the first frame update
    void Awake()
    {
        rig = GetComponent<Rig>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        DialogueUI = GameObject.Find("BossSceneDialogueBox").GetComponent<BossSceneDialogue>();
    }

    // Update is called once per frame
    void Update()
    {
        if (player.IsDie || DialogueUI.IsTalk) return;

        if (player.IsAiming)
        {
            if (player.IsCriticalHit)
            {
                rig.weight = 0f;

                IsRightKeyDown = Input.GetMouseButton(1);

                if(!player.IsCriticalHit && IsRightKeyDown) 
                {
                    rig.weight = 1f;
                }
            }
        }
        else
        {
            rig.weight = 0f;
        }
    }
}
