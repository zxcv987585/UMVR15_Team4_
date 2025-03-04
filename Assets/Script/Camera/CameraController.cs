﻿using System;
using System.ComponentModel;
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

    private Vector3 aimTargetPosition;
    //原本的跟隨目標
    private Transform originalTarget;

    //最小與最大攝影機仰角程度
    float MinVerticalAngle = -15;
    float MaxVerticalAngle = 35;
    //攝影機與玩家的距離
    float CameraToTargetDistance = 3.5f;
    float Mouse_x = 0;
    float Mouse_y = 30;
    Vector3 smoothVelocity = Vector3.zero;

    private InputController input;
    private PlayerController player;

    private bool isAiming = false;

    private void Awake()
    {
        input = GameManagerSingleton.Instance.inputControl;
    }

    private void Start()
    {
        player = GameObject.FindWithTag("Player").GetComponent<PlayerController>();
        GameInput.Instance.OnAimAction += SetAim;

        originalTarget = target;
        if (player != null)
        {
            playerTransfrom = player.transform;
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

            if (AimTarget != null)
            {
                Vector3 cameraForward = Camera.main.transform.forward;
                AimTarget.position = Camera.main.transform.position + cameraForward * 10f;
            }
        }
        else
        {
            CameraToTargetDistance = 3.5f;
        }

        if (player.LockTarget != null)
        {
            LockTransfrom = player.LockTarget;
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
        else
        {
            LockTransfrom = null;
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