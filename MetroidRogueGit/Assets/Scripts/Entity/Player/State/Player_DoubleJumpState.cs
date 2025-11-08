using UnityEngine;

public class Player_DoubleJumpState : Player_AiredState
{
    public Player_DoubleJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
   

    public override void Enter()
    {
        base.Enter();
        player.canDoubleJump = false;

        player.SetVelocity(rb.linearVelocity.x, player.doubleJumpForce);
   
    }

    public override void Update()
    {
        base.Update();
        CancelDoubleJumpIfNeeded();

        if (rb.linearVelocity.y< 0 && stateMachine.currentState != player.jumpAttackState)
        {
                stateMachine.ChangeState(player.fallState);
        }
          
    }
    public override void Exit()
    {
        base.Exit();
    
    }

    private void CancelDoubleJumpIfNeeded()
    {
        if (player.wallDetected)
        {
            if (player.groundDetected)
                stateMachine.ChangeState(player.idleState);
            else
                stateMachine.ChangeState(player.wallSlideState);
        }
    }

}
