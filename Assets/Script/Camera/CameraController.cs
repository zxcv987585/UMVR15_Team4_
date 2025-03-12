using System.Collections;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("攝影機跟隨的目標")]
    [SerializeField] Transform target;

    [Header("玩家鎖定模式時要看向的目標")]
    [SerializeField] Transform LockTransfrom;

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

    // 用來控制普通與瞄準模式之間的平滑過渡
    private float transitionValue = 0f;
    [Header("轉換速度")]
    [SerializeField] float transitionSpeed = 10f;

    // 攝影機預設的距離
    private float DefaultCameraToTargetDistance;
    // 攝影機上一幀的距離
    private float PreviousCameraToTargetDistance;

    // 最小與最大攝影機仰角程度
    float MinVerticalAngle = -15;
    float MaxVerticalAngle = 35;
    // 攝影機與玩家的距離
    float CameraToTargetDistance = 4f;
    float Mouse_x = 0;
    float Mouse_y = 30;
    //存儲絲滑移動量的變數
    Vector3 smoothVelocity = Vector3.zero;
    //存儲震動偏移量的變數
    Vector3 shakeOffset = Vector3.zero;

    private InputController input;
    private PlayerController player;

    private bool isAiming = false;
    private bool isLocked => player?.LockTarget != null;

    [Header("碰撞檢測")]
    [SerializeField] LayerMask collisionLayers;
    [SerializeField] float collisionRadius = 0.2f;
    [SerializeField] float collisionOffset = 0.2f;

    private void Awake()
    {
        if (FindObjectsOfType<CameraController>().Length > 1)
        {
            Destroy(gameObject);
            return;
        }
        DontDestroyOnLoad(gameObject);
    }
    private void Start()
    {
        input = GameManagerSingleton.Instance.InputControl;
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        GameInput.Instance.OnAimAction += SetAim;

        DefaultCameraToTargetDistance = CameraToTargetDistance;
    }

    private void LateUpdate()
    {
        HandleCameraRotation();

        if (!isLocked)
        {
            // 根據是否瞄準，計算過渡參數（0~1）
            float targetTransition = isAiming ? 1f : 0f;
            transitionValue = Mathf.Lerp(transitionValue, targetTransition, Time.deltaTime * transitionSpeed);

            // 普通模式下的參數
            Vector3 normalPosition = target.position + Vector3.up * offset.y;
            Quaternion normalRotation = Quaternion.Euler(Mouse_y, Mouse_x, 0);
            float normalCamDistance = CameraToTargetDistance;

            // 瞄準模式下的參數（可根據需求調整）
            Vector3 aimPosition = target.position + target.right * AimOffset.x + target.up * AimOffset.y;
            Quaternion aimRotation = Quaternion.Euler(Mouse_y, Mouse_x + 15f, 0);
            float aimCamDistance = AimOffset.z;

            // 插值混合位置、旋轉與距離
            Vector3 blendedPosition = Vector3.Lerp(normalPosition, aimPosition, transitionValue);
            Quaternion blendedRotation = Quaternion.Slerp(normalRotation, aimRotation, transitionValue);
            float blendedDistance = Mathf.Lerp(normalCamDistance, aimCamDistance, transitionValue);

            if (isAiming && AimTarget != null)
            {
                if (player.IsDie) return;
                Vector3 cameraForward = Camera.main.transform.forward;
                AimTarget.position = Camera.main.transform.position + cameraForward * 10f;
            }

            // 計算期望的相機位置
            Vector3 desiredCameraPos = blendedPosition + blendedRotation * new Vector3(0, 0, -blendedDistance);

            // 碰撞檢測：從 blendedPosition 發射一個球形射線
            Vector3 direction = (desiredCameraPos - blendedPosition).normalized;
            float adjustedDistance = blendedDistance;
            RaycastHit hit;
            if (Physics.SphereCast(blendedPosition, collisionRadius, direction, out hit, blendedDistance, collisionLayers))
            {
                adjustedDistance = hit.distance - collisionOffset;
                adjustedDistance = Mathf.Clamp(adjustedDistance, 0.1f, blendedDistance);
                desiredCameraPos = blendedPosition + blendedRotation * new Vector3(0, 0, -adjustedDistance);
            }

            // 平滑更新相機位置，最後加上 shakeOffset 來達成震動效果
            Vector3 finalPosition = Vector3.SmoothDamp(transform.position, desiredCameraPos, ref smoothVelocity, SmoothTime);
            finalPosition += shakeOffset;
            transform.position = finalPosition;
            transform.rotation = blendedRotation;
        }

        if (isLocked && !isAiming)
        {
            HandleLockMode();
        }
    }

    // 處理滑鼠輸入來旋轉攝影機
    private void HandleCameraRotation()
    {
        if (isLocked && !isAiming) return;
        Mouse_x += input.GetMouseXAxis() * sensitivity_x;
        Mouse_y -= input.GetMouseYAxis() * sensitivity_y;
        Mouse_y = Mathf.Clamp(Mouse_y, MinVerticalAngle, MaxVerticalAngle);
    }

    // 鎖定模式下的相機處理邏輯
    private void HandleLockMode()
    {
        LockTransfrom = player.LockTarget;
        if (LockTransfrom == null || isAiming) return;

        Vector3 targetPosition = target.position + Vector3.up * 1.3f;
        Vector3 desiredCameraPos = targetPosition + transform.rotation * new Vector3(0, 0, -CameraToTargetDistance);

        int WallLayer = LayerMask.GetMask("Wall");
        float targetDistance = DefaultCameraToTargetDistance;
        if (Physics.Raycast(targetPosition, (desiredCameraPos - targetPosition).normalized, out RaycastHit hit, CameraToTargetDistance, WallLayer))
        {
            targetDistance = Mathf.Lerp(PreviousCameraToTargetDistance, hit.distance - 0.2f, Time.deltaTime * 10f);
        }
        else
        {
            targetDistance = Mathf.Lerp(PreviousCameraToTargetDistance, DefaultCameraToTargetDistance, Time.deltaTime * 3f);
        }

        CameraToTargetDistance = targetDistance;
        PreviousCameraToTargetDistance = CameraToTargetDistance;

        Vector3 finalPosition = targetPosition + transform.rotation * new Vector3(0, 0, -CameraToTargetDistance);
        // 加入震動偏移量
        finalPosition += shakeOffset;
        transform.position = Vector3.SmoothDamp(transform.position, finalPosition, ref smoothVelocity, SmoothTime);

        Vector3 rotationDirection = LockTransfrom.position - transform.position;
        rotationDirection.y = 0f;
        Quaternion targetRotation = Quaternion.LookRotation(rotationDirection);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, LockOnTargetFollowSpeed);

        // 更新滑鼠角度以延續後續普通視角方向
        Vector3 currentEuler = transform.rotation.eulerAngles;
        Mouse_x = currentEuler.y;
        Mouse_y = currentEuler.x;
    }

    // 取得瞄準輸入
    private void SetAim(bool isAiming)
    {
        if (player.IsDie) return;
        this.isAiming = isAiming;
    }

    // BOSS 傳送門進入後的角度調整
    public void SetCameraRotation(float yaw, float pitch)
    {
        Mouse_x = yaw;
        Mouse_y = Mathf.Clamp(pitch, MinVerticalAngle, MaxVerticalAngle);
    }

    // 攻擊與特殊技能的震動效果，僅更新 shakeOffset 變數
    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Debug.Log("開始震動!");
        float elapsed = 0f;
        while (elapsed < duration)
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            shakeOffset = new Vector3(offsetX, offsetY, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }
        shakeOffset = Vector3.zero;
    }
}