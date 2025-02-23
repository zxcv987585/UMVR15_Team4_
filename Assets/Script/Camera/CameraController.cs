using System;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("攝影機跟隨的目標")]
    [SerializeField] Transform target;
    private Transform originalTarget;

    [Header("玩家瞄準模式時要跟隨的對象")]
    [SerializeField] Transform playerTransfrom;

    [Header("水平靈敏度")]
    [SerializeField] float sensitivity_x = 2;

    [Header("垂直靈敏度")]
    [SerializeField] float sensitivity_y = 2;

    [Header("offset")]
    [SerializeField] Vector3 offset;

    [Header("瞄準狀態下的offset")]
    [SerializeField] Vector3 AimOffset;

    [Header("攝影機的延遲跟隨時間")]
    [SerializeField] float SmoothTime = 0.01f;

    float MinVerticalAngle = -10;
    float MaxVerticalAngle = 30;
    float CameraToTargetDistance = 3.5f;
    float Mouse_x = 0;
    float Mouse_y = 30;
    Vector3 smoothVelocity = Vector3.zero;

    InputController input;

    private bool isAiming = false;
    private PlayerController playerController;

    private void Awake()
    {
        input = GameManagerSingleton.Instance.inputControl;
    }

    private void Start()
    {
        playerController = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        GameInput.Instance.OnAimAction += SetAim;

        originalTarget = target;
        if (playerController != null)
        {
            playerTransfrom = playerController.transform;
        }
    }

    private void SetAim(bool isAiming)
    {
        this.isAiming = isAiming;

        target = isAiming ? playerTransfrom : originalTarget;
    }

    private void LateUpdate()
    {
        //處裡滑鼠輸入來旋轉攝影機
        Mouse_x += input.GetMouseXAxis() * sensitivity_x;
        Mouse_y -= input.GetMouseYAxis() * sensitivity_y;
        Mouse_y = Math.Clamp(Mouse_y, MinVerticalAngle, MaxVerticalAngle);

        //計算選轉角度
        float aimAngleoffset = isAiming ? 15f : 0f;
        Quaternion rotation = Quaternion.Euler(Mouse_y, Mouse_x + aimAngleoffset, 0);

        //計算目標位置
        Vector3 TargetPosition = target.position + Vector3.up * offset.y;

        //進入瞄準模式調整攝影機位置
        if (isAiming)
        {
            TargetPosition += target.right * AimOffset.x;
            TargetPosition += target.up * AimOffset.y;
            CameraToTargetDistance = AimOffset.z;
        }
        else
        {
            CameraToTargetDistance = 3.5f;
        }

        Vector3 desiredCameraPos = TargetPosition + rotation * new Vector3(0, 0, -CameraToTargetDistance);

        //設置RayCast來檢測碰撞
        Vector3 direction = (desiredCameraPos - TargetPosition).normalized;
        float distance = CameraToTargetDistance;
        int WallLayer = LayerMask.GetMask("Wall");

        if (Physics.Raycast(target.position, direction, out RaycastHit hit, CameraToTargetDistance, WallLayer))
        {
            distance = hit.distance - 0.5f;
        }

        Vector3 finalPosition = TargetPosition + rotation * new Vector3(0, 0, -distance);

        //設置攝影機位置
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref smoothVelocity, SmoothTime);
        transform.rotation = rotation;
    }
}