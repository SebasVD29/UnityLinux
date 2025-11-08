using UnityEngine;

public class Player_JumpState : Player_AiredState
{
    private float jumpDelayTimer;
    public Player_JumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    public override void Enter()
    {
        base.Enter();
        player.SetVelocity(rb.linearVelocity.x, player.jumpForce);
        jumpDelayTimer = 0.1f;
        player.hasJumped = true;
    }


    public override void Update()
    {
        base.Update();


        if (jumpDelayTimer > 0)
            jumpDelayTimer -= Time.deltaTime;


            if (rb.linearVelocity.y < 0 && stateMachine.currentState != player.jumpAttackState)
            stateMachine.ChangeState(player.fallState);

        if (jumpDelayTimer <= 0  && input.Player.Jump.WasPressedThisFrame() && CanDobleJump())
        {
            stateMachine.ChangeState(player.doubleJumpState);
        }
    }
    public override void Exit()
    {
        base.Exit();

        
    }
    

}
