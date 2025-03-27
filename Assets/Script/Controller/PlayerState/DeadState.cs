
using System;

public class DeadState : PlayerState
{
    public DeadState(PlayerStateMachine stateMachine, PlayerController player) : base(stateMachine, player) { }

    public event Action Dead;

    public override void Enter() 
    {
        Dead?.Invoke();
    }

    public override void Update(){}

    public override void Move() { }
}
