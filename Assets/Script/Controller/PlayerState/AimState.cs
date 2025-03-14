using System;
using UnityEngine;

public class AimState : PlayerState
{
    private float AimSpeedModifier = 0.5f;
    private WeaponManager weaponManager;

    public Action<bool> OnAim;
    public Action<float, float> OnAimMove;

    public AimState(PlayerStateMachine stateMachine, PlayerController player) : base(stateMachine, player)
    {
        weaponManager = player.GetComponent<WeaponManager>();
    }

    public override void Enter()
    {
        if (player.IsDie) return;

        weaponManager.SwitchWeapon(WeaponManager.WeaponType.Gun);
        OnAim?.Invoke(true);
        Vector3 cameraForward = player.GetCurrentCameraForward();
        player.transform.rotation = Quaternion.LookRotation(cameraForward, Vector3.up);
    }

    public override void Update()
    {
        if (!player.IsAiming)
        {
            StateMachine.ChangeState(player.moveState);
            OnAim?.Invoke(false);
            return;
        }
        if (!player.IsAiming && player.GetMoveInput().sqrMagnitude < 0.01f)
        {
            StateMachine.ChangeState(player.idleState);
            OnAim?.Invoke(false);
            return;
        }
        if (player.IsDie)
        {
            StateMachine.ChangeState(player.deadState);
            OnAim?.Invoke(false);
            return;
        }

        RotateToCamera();
        Move();
    }

    private void RotateToCamera()
    {
        Vector3 cameraForward = player.GetCurrentCameraForward();
        player.transform.rotation = Quaternion.LookRotation(cameraForward, Vector3.up);
    }

    public override void Move()
    {
        Vector3 moveDirection = player.GetMoveInput().normalized;

        //計算角色相對於攝影機的移動方向
        Vector3 cameraForward = player.GetCurrentCameraForward();
        Vector3 cameraRight = player.GetCurrentCameraRight();

        //根據相機方向調整角色的移動方向
        Vector3 targetDirection = cameraForward * moveDirection.z + cameraRight * moveDirection.x;

        float currentSpeed = player.playerData.MoveSpeed * AimSpeedModifier;
        player.MoveWithOutRotation(targetDirection, currentSpeed);

        //計算AnimatorBlendTree的參數
        float moveX = Vector3.Dot(targetDirection, cameraRight);
        float moveY = Vector3.Dot(targetDirection, cameraForward);

        OnAimMove?.Invoke(moveX, moveY);
    }

    public override void Exit()
    {
        weaponManager.EquipPreviousWeapon();
    }
}
