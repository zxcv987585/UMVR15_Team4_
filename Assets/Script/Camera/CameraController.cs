using System;
using System.ComponentModel;
using TMPro;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("攝影機跟隨的目標")]
    [SerializeField] Transform target;

    [Header("玩家瞄準模式時要跟隨的對象")]
    [SerializeField] Transform playerTransfrom;

    [Header("玩家鎖定模式時要看向的目標")]
    [SerializeField] Transform LockTransfrom;

    [Header("玩家鎖定模式時要跟隨的對象")]
    [SerializeField] Transform CameraPivotTransform;

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

    [Header("瞄準狀態下角色上半身的跟隨物")]
    [SerializeField] Transform AimTarget;

    [Header("鎖定狀態攝影機的移動速度")]
    [SerializeField] float LockOnTargetFollowSpeed;

    //攝影機切換前的位置
    private Vector3 OriginCameraPosition;
    //原本的跟隨目標
    private Transform originalTarget;
    //攝影機預設的距離
    private float DefaultCameraToTargetDistance;
    //攝影機上一幀的距離
    private float PreviousCameraToTargetDistance;

    //最小與最大攝影機仰角程度
    float MinVerticalAngle = -15;
    float MaxVerticalAngle = 35;
    //攝影機與玩家的距離
    float CameraToTargetDistance = 4.5f;
    float Mouse_x = 0;
    float Mouse_y = 30;
    Vector3 smoothVelocity = Vector3.zero;

    private InputController input;
    private PlayerController player;

    private bool isAiming = false;
    private bool isLocked => player?.LockTarget != null;

    private void Start()
    {
        input = GameManagerSingleton.Instance.InputControl;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        GameInput.Instance.OnAimAction += SetAim;

        originalTarget = target;
        if (player != null)
        {
            playerTransfrom = player.transform;
        }

        DefaultCameraToTargetDistance = CameraToTargetDistance;
    }

    private void LateUpdate()
    {
        HandleCameraRotation();

        //計算選轉角度
        float aimAngleoffset = isAiming ? 15f : 0f;
        Quaternion rotation = Quaternion.Euler(Mouse_y, Mouse_x + aimAngleoffset, 0);

        //計算目標位置
        Vector3 TargetPosition = target.position + Vector3.up * offset.y;

        //進入瞄準模式調整攝影機位置
        if (isAiming)
        {
            HandleAimMode();
        }
        else if (isLocked && !isAiming)
        {
            HandleLockMode();
        }
        else
        {
            HandleNormalFollow();
        }
    }

    //攝影機輸入邏輯
    private void HandleCameraRotation()
    {
        if (isLocked && !isAiming) return;
        //處裡滑鼠輸入來旋轉攝影機
        Mouse_x += input.GetMouseXAxis() * sensitivity_x;
        Mouse_y -= input.GetMouseYAxis() * sensitivity_y;
        Mouse_y = Math.Clamp(Mouse_y, MinVerticalAngle, MaxVerticalAngle);
    }

    //平常的攝影機跟隨狀態
    private void HandleNormalFollow()
    {
        CameraToTargetDistance = 3.5f;
        UpdateCameraPostion(target.position + Vector3.up * offset.y, Quaternion.Euler(Mouse_y, Mouse_x, 0));
    }

    //瞄準模式下的攝影機邏輯
    private void HandleAimMode()
    {
        Vector3 TargetPosition = playerTransfrom.position;
        TargetPosition += playerTransfrom.right * AimOffset.x;
        TargetPosition += playerTransfrom.up * AimOffset.y;
        CameraToTargetDistance = AimOffset.z;

        if (AimTarget != null)
        {
            Vector3 cameraForward = Camera.main.transform.forward;
            AimTarget.position = Camera.main.transform.position + cameraForward * 10f;
        }

        UpdateCameraPostion(TargetPosition, Quaternion.Euler(Mouse_y, Mouse_x + 15f, 0));
    }

    //鎖定模式下的攝影機邏輯
    private void HandleLockMode()
    {
        LockTransfrom = player.LockTarget;
        if (LockTransfrom == null || isAiming) return;

        Vector3 targetPoition = CameraPivotTransform.position + Vector3.up * 1.3f;
        Vector3 desiredCameraPos = targetPoition + transform.rotation * new Vector3(0, 0, -CameraToTargetDistance);

        int WallLayer = LayerMask.GetMask("Wall");
        float targetDistance = DefaultCameraToTargetDistance;
        if (Physics.Raycast(targetPoition, (desiredCameraPos - targetPoition).normalized, out RaycastHit hit, CameraToTargetDistance, WallLayer))
        {
            targetDistance = Mathf.Lerp(PreviousCameraToTargetDistance, hit.distance - 0.2f, Time.deltaTime * 10f);
        }
        else
        {
            targetDistance = Mathf.Lerp(PreviousCameraToTargetDistance, DefaultCameraToTargetDistance, Time.deltaTime * 3f);
        }

        CameraToTargetDistance = targetDistance;
        PreviousCameraToTargetDistance = CameraToTargetDistance;

        Vector3 finalPosition = targetPoition + transform.rotation * new Vector3(0, 0, -CameraToTargetDistance);
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref smoothVelocity, SmoothTime);

        Vector3 rotationDirection = LockTransfrom.transform.position - transform.position;
        rotationDirection.Normalize();
        rotationDirection.y = 0f;

        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, LockOnTargetFollowSpeed);

        rotationDirection = LockTransfrom.position - CameraPivotTransform.position;
        rotationDirection.Normalize();

        targetRotation = Quaternion.LookRotation(rotationDirection);
        CameraPivotTransform.transform.rotation = Quaternion.Slerp(CameraPivotTransform.rotation, targetRotation, LockOnTargetFollowSpeed);
    }

    //更新攝影機的位置
    private void UpdateCameraPostion(Vector3 targetPosition, Quaternion rotation)
    {
        Vector3 desiredCameraPos = targetPosition + rotation * new Vector3(0, 0, -CameraToTargetDistance);

        //設置RayCast來檢測碰撞
        Vector3 direction = (desiredCameraPos - targetPosition).normalized;
        float distance = CameraToTargetDistance;
        int WallLayer = LayerMask.GetMask("Wall");

        if (Physics.Raycast(target.position, direction, out RaycastHit hit, CameraToTargetDistance, WallLayer))
        {
            distance = hit.distance - 0.5f;
        }

        Vector3 finalPosition = targetPosition + rotation * new Vector3(0, 0, -distance);

        //設置攝影機位置
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref smoothVelocity, SmoothTime);
        transform.rotation = rotation;
    }

    //獲取瞄準輸入
    private void SetAim(bool isAiming)
    {
        this.isAiming = isAiming;

        target = isAiming ? playerTransfrom : originalTarget;
    }
}