using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowSmooth : MonoBehaviour
{
    [Tooltip("跟隨的Player目標")]
    [SerializeField] Transform Player;
    [Tooltip("跟目標的最大距離")]
    [SerializeField] float DistanceToTarget;
    [Tooltip("起始高度")]
    [SerializeField] float StartHight;
    [Tooltip("平滑的移動時間")]
    private float SmoothTime = 0.13f;

    float Offset_Y;

    Vector3 SmoothPosition = Vector3.zero;
    Vector3 CurrentVelocity = Vector3.zero;

    private void Awake()
    {
        transform.position = Player.position + Vector3.up * StartHight;
        Offset_Y = StartHight;
    }

    private void Start()
    {
        GameInput.Instance.OnAimAction += SetIsAiming;
    }

    private void SetIsAiming(bool isAim)
    {
        if (isAim) 
        {
            SmoothTime = 0.08f;
        }
        else
        {
            SmoothTime = 0.13f;
        }
    }

    private void LateUpdate()
    {
        if (CheckDistance())
        {
            SmoothPosition = Vector3.SmoothDamp(transform.position, Player.position + Vector3.up * Offset_Y, ref CurrentVelocity, SmoothTime);
            transform.position = SmoothPosition;
        }
    }

    //檢查與目標的距離
    private bool CheckDistance()
    {
        return Vector3.Distance(transform.position, Player.position) > DistanceToTarget;
    }
}
