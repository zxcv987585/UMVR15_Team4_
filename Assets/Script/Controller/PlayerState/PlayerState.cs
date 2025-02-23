using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState
{
    protected PlayerStateMachine StateMachine;
    protected PlayerController player;

    public PlayerState(PlayerStateMachine stateMachine, PlayerController player)
    {
        this.StateMachine = stateMachine;
        this.player = player;
    }

    public virtual void Enter() { }
    public virtual void Update() { }
    public virtual void Exit() { }

    public abstract void Move();
}