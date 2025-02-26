using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleState : PlayerState
{
    public Action<bool> IsIdle;

    private float idleTimer = 0f;
    private float idleThreshold = 0.1f;

    public IdleState(PlayerStateMachine StateMachine, PlayerController player) : base(StateMachine, player) { }

    public override void Enter()
    {
        IsIdle?.Invoke(true);
        idleTimer = 0f;
    }
    public override void Update()
    {
        if (player == null)
        {
            Debug.LogError("Player is not assigned in IdleState!");
            return;
        }
        if (player.GetMoveInput().sqrMagnitude > 0.01f)
        {
            idleTimer += Time.deltaTime;
            if (idleTimer >= idleThreshold)
            {
                StateMachine.ChangeState(player.moveState);
                IsIdle?.Invoke(false);
            }
        }
        else if (player.isAiming)
        {
            StateMachine.ChangeState(player.aimState);
            IsIdle?.Invoke(false);
        }
        else if (player.IsDie)
        {
            StateMachine.ChangeState(player.deadState);
            IsIdle?.Invoke(false);
        }
        else
        {
            idleTimer = 0f;
        }
        if (player.isAttack)
        {
            StateMachine.ChangeState(player.fightState);
            IsIdle?.Invoke(false);
        }
    }

    public override void Move() { }
}