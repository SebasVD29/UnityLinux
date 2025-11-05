using UnityEngine;

public class Player_WallJumpState : PlayerState
{
    public Player_WallJumpState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {

    }

   

    public override void Enter()
    {
        base.Enter();
        player.canDoubleJump = true;
        player.canAirDash = true;

        WallJump();

        //Debug.Log("WALL JUMP arriba");

    }

    public override void Update()
    {
        base.Update();
       


        if (rb.linearVelocity.y < 0)
            stateMachine.ChangeState(player.fallState);

        if (player.wallDetected)
            stateMachine.ChangeState(player.wallSlideState);

        //if (input.Player.Jump.WasPressedThisFrame() && CanDobleJump())
        //{
        //    stateMachine.ChangeState(player.doubleJumpState);
        //}

    }

    public void WallJump()
    {
        if (player.moveInput.x == player.facingDir)
        {
            float curveX = player.wallJumpForce.x * 0.3f; // menor fuerza lateral
            float curveY = player.wallJumpForce.y * 1.4f; // mayor fuerza vertical
            // Salto más vertical, casi pegado al muro
            player.SetVelocity(curveX * -player.facingDir, curveY);
        }
        else 
        {
            // Salta más horizontal, alejándose del muro
            player.SetVelocity(player.wallJumpForce.x * -player.facingDir, player.wallJumpForce.y);
        }
    }
}
