using UnityEngine;

public class Player_FallState : Player_AiredState
{
    public Player_FallState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
      

    }
    public override void Update()
    {
        base.Update();

        if (input.Player.Jump.WasPressedThisFrame() && CanDobleJump())
        {
            stateMachine.ChangeState(player.doubleJumpState);
        }

        if (player.groundDetected)
            stateMachine.ChangeState(player.idleState);

        if (player.wallDetected)
            stateMachine.ChangeState(player.wallSlideState);

        
    }
}
