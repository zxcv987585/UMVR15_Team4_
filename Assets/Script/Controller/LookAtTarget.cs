using UnityEngine;
using UnityEngine.Animations.Rigging;

public class LookAtTarget : MonoBehaviour
{
    private Rig rig;
    private float targetWeight;

    private PlayerController player;

    // Start is called before the first frame update
    void Awake()
    {
        rig = GetComponent<Rig>();
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {

        if (player.IsAiming)
        {
            rig.weight = 1f;
        }
        else
        {
            rig.weight = 0f;
        }
    }
}
