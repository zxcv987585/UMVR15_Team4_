using System;
using UnityEngine;
using UnityEngine.AI;

public class MoveState : PlayerState
{
    public Action<bool> IsMoving;
    public Action<bool> IsRun;

    public MoveState(PlayerStateMachine stateMachine, PlayerController player) : base(stateMachine, player) { }

    public override void Enter()
    {
        IsMoving?.Invoke(true);
        IsRun?.Invoke(player.isRun);
    }

    public override void Update()
    {
        // 如果玩家沒有輸入移動，則切換到 Idle 狀態
        if (player.GetMoveInput().sqrMagnitude < 0.01f)
        {
            StateMachine.ChangeState(player.idleState);
            IsMoving?.Invoke(false);
            IsRun?.Invoke(false);
            return;
        }
        if (player.isAiming)
        {
            StateMachine.ChangeState(player.aimState);
            IsMoving?.Invoke(false);
            IsRun?.Invoke(false);
            return;
        }
        if (player.IsDie)
        {
            StateMachine.ChangeState(player.deadState);
            IsMoving?.Invoke(false);
            IsRun?.Invoke(false);
            return;
        }
        if (player.isAttack)
        {
            StateMachine.ChangeState(player.fightState);
            IsMoving?.Invoke(false);
            IsRun?.Invoke(false);
            return;
        }
        if (player.isDash)
        {
            StateMachine.ChangeState(player.dashState);
            IsMoving?.Invoke(false);
            IsRun?.Invoke(false);
            return;
        }
        if (player.isHit)
        {
            IsMoving?.Invoke(false);
            IsRun?.Invoke(false);
            return;
        }

        Move();
    }

    public override void Move()
    {
        Vector3 moveDirection = player.GetMoveInput().normalized;

        // 計算角色相對於攝影機的移動方向
        Vector3 cameraForward = player.GetCurrentCameraForward();
        Vector3 cameraRight = player.GetCurrentCameraRight();

        // 根據相機方向調整角色的移動方向
        Vector3 targetDirection = cameraForward * moveDirection.z + cameraRight * moveDirection.x;

        bool isSprinting = player.isRun;
        // 根據玩家是否正在跑步來調整速度
        float currentSpeed = player.isRun ? player.playerData.MoveSpeed * player.playerData.SprintSpeedModifier : player.playerData.MoveSpeed;

        IsRun?.Invoke(isSprinting);
        IsMoving?.Invoke(!isSprinting);

        player.MoveCharacter(targetDirection, currentSpeed);
    }
}
