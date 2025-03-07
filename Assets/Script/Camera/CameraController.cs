using System.Collections;
using TMPro;
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
    Vector3 smoothVelocity = Vector3.zero;

    private InputController input;
    private PlayerController player;

    private bool isAiming = false;
    private bool isLocked => player?.LockTarget != null;

    [Header("碰撞檢測")]
    [SerializeField] LayerMask collisionLayers;
    [SerializeField] float collisionRadius = 0.2f;
    [SerializeField] float collisionOffset = 0.2f;

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
            // 計算轉換參數，根據是否瞄準，目標值分別為1或0
            float targetTransition = isAiming ? 1f : 0f;
            transitionValue = Mathf.Lerp(transitionValue, targetTransition, Time.deltaTime * transitionSpeed);

            // 普通模式下的參數
            Vector3 normalPosition = target.position + Vector3.up * offset.y;
            Quaternion normalRotation = Quaternion.Euler(Mouse_y, Mouse_x, 0);
            float normalCamDistance = CameraToTargetDistance;

            // 瞄準模式下的參數（根據你的程式邏輯計算）
            Vector3 aimPosition = target.position + target.right * AimOffset.x + target.up * AimOffset.y;
            // 旋轉上加個偏移，例如額外偏移15度（依需求調整）
            Quaternion aimRotation = Quaternion.Euler(Mouse_y, Mouse_x + 15f, 0);
            float aimCamDistance = AimOffset.z;

            // 混合參數：位置、旋轉與攝影機距離都使用過渡參數進行插值
            Vector3 blendedPosition = Vector3.Lerp(normalPosition, aimPosition, transitionValue);
            Quaternion blendedRotation = Quaternion.Slerp(normalRotation, aimRotation, transitionValue);
            float blendedDistance = Mathf.Lerp(normalCamDistance, aimCamDistance, transitionValue);

            if (isAiming && AimTarget != null)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                AimTarget.position = Camera.main.transform.position + cameraForward * 10f;
            }

            // 最後更新攝影機位置，加入碰撞檢測邏輯
            UpdateCameraPosition(blendedPosition, blendedRotation, blendedDistance);
        }

        if (isLocked && !isAiming)
        {
            HandleLockMode();
        }
    }

    // 攝影機輸入邏輯
    private void HandleCameraRotation()
    {
        if (isLocked && !isAiming) return;
        // 處理滑鼠輸入來旋轉攝影機
        Mouse_x += input.GetMouseXAxis() * sensitivity_x;
        Mouse_y -= input.GetMouseYAxis() * sensitivity_y;
        Mouse_y = Mathf.Clamp(Mouse_y, MinVerticalAngle, MaxVerticalAngle);
    }

    // 更新攝影機位置的方法
    private void UpdateCameraPosition(Vector3 targetPosition, Quaternion rotation, float distance)
    {
        // 期望的攝影機位置：從目標位置向後一定距離
        Vector3 desiredCameraPos = targetPosition + rotation * new Vector3(0, 0, -distance);

        // 碰撞檢測：從 targetPosition 發射一個球形射線
        Vector3 direction = (desiredCameraPos - targetPosition).normalized;
        float adjustedDistance = distance;
        RaycastHit hit;
        if (Physics.SphereCast(targetPosition, collisionRadius, direction, out hit, distance, collisionLayers))
        {
            // 碰撞到障礙物時，將攝影機距離調整為碰撞點距離，並留下一些間隙
            adjustedDistance = hit.distance - collisionOffset;
            adjustedDistance = Mathf.Clamp(adjustedDistance, 0.1f, distance);
            desiredCameraPos = targetPosition + rotation * new Vector3(0, 0, -adjustedDistance);
        }

        Vector3 finalPosition = Vector3.SmoothDamp(transform.position, desiredCameraPos, ref smoothVelocity, SmoothTime);
        transform.position = finalPosition;
        transform.rotation = rotation;
    }

    // 鎖定模式下的攝影機邏輯
    private void HandleLockMode()
    {
        LockTransfrom = player.LockTarget;
        if (LockTransfrom == null || isAiming) return;

        Vector3 targetPoition = target.position + Vector3.up * 1.3f;
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

        rotationDirection = LockTransfrom.position - target.position;
        rotationDirection.Normalize();

        // 更新滑鼠角度，讓後續切換回普通視角時能夠延續目前的方向
        Vector3 currentEuler = transform.rotation.eulerAngles;
        Mouse_x = currentEuler.y;
        Mouse_y = currentEuler.x;
    }

    // 獲取瞄準輸入
    private void SetAim(bool isAiming)
    {
        this.isAiming = isAiming;
    }

    //進入BOSS傳送門後的角度修正
    public void SetCameraRotation(float yaw, float pitch)
    {
        Mouse_x = yaw;
        Mouse_y = Mathf.Clamp(pitch, MinVerticalAngle, MaxVerticalAngle);
    }

    public IEnumerator ShakeCamera(float duration, float magnitude)
    {
        Vector3 originalPos = transform.position;
        float elapsed = 0f;

        while (elapsed < duration) 
        {
            float offsetX = Random.Range(-1f, 1f) * magnitude;
            float offsetY = Random.Range(-1f, 1f) * magnitude;
            transform.position = originalPos + new Vector3(offsetX, offsetY, 0f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPos;
    }
}