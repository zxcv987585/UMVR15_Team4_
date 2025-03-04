using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState
{
    private float dashTimer;
    private Vector3 dashDirection;

    public Action<String> Dash;
    public Action<bool> DashReset;
    public Action<bool> ForceIdle;

    public DashState(PlayerStateMachine stateMachine, PlayerController player) : base(stateMachine, player) { }

    public override void Enter()
    {
        player.Invincible = true;
        DashReset?.Invoke(false);
        Dash?.Invoke("Dash");
        dashTimer = player.playerData.DashDuration;

        Vector3 inputDirection = player.GetMoveInput().normalized;

        //計算角色相對於攝影機的移動方向
        Vector3 cameraForward = player.GetCurrentCameraForward();
        Vector3 cameraRight = player.GetCurrentCameraRight();

        if (inputDirection == Vector3.zero) 
        { 
            dashDirection = player.transform.forward;
        }
        else
        {
            //計算Dash方向，讓他與相機角度對齊
            dashDirection = (cameraForward * inputDirection.z + cameraRight * inputDirection.x).normalized;
        }

        player.SetRotation(dashDirection);

        player.Velocity = dashDirection * player.playerData.DashSpeed;
    }

    public override void Update()
    {
        if (dashTimer > 0f)
        {
            dashTimer -= Time.deltaTime;
            player.controller.SimpleMove(player.Velocity);
            return;
        }
        player.isDash = false;
        player.Invincible = false;
        StateMachine.ChangeState(player.idleState);
        ForceIdle?.Invoke(true);
        player.Velocity = Vector3.zero;
        DashReset?.Invoke(true);  
    }

    public override void Move() { }
}