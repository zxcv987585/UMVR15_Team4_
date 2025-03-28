using UnityEngine;
using UnityEngine.UI;

public class LockOnIndicator : MonoBehaviour
{
    //指向UI上的白點
    public RectTransform indicatorRect;
    //參考攝影機
    public Camera Maincamera;
    //參考玩家
    public PlayerController playerController;
    //指向被鎖定的敵人
    public Transform Enemy;
    //補正白點位置
    public float verticalOffset = 1.0f;

    // Start is called before the first frame update
    void Awake()
    {
        Maincamera = Camera.main;
        playerController = FindAnyObjectByType<PlayerController>();
        indicatorRect.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        Enemy = playerController.LockTarget;

        if (Enemy != null) 
        {
            indicatorRect.gameObject.SetActive(true);
            Collider enemyCollider = Enemy.GetComponent<Collider>();
            Vector3 targetPosition = Enemy.position; // 預設為物件位置

            if (enemyCollider != null)
            {
                // 使用 Collider 的中心點
                targetPosition = enemyCollider.bounds.center;
            }

            Vector3 screenPos = Maincamera.WorldToScreenPoint(targetPosition);
            indicatorRect.position = screenPos;
        }
        else
        {
            indicatorRect.gameObject.SetActive (false);
        }
    }
}
