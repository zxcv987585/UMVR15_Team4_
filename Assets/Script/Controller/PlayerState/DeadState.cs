
public class DeadState : PlayerState
{
    public DeadState(PlayerStateMachine stateMachine, PlayerController player) : base(stateMachine, player) { }

    public override void Enter() {}

    public override void Update(){}

    public override void Move() { }
}
