using UnityEngine;

public class Player_GroundedState : PlayerState
{
    public Player_GroundedState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }

    
    public override void Enter()
    {
        base.Enter();
        player.canDoubleJump = true;
        player.canAirDash = true;

    }

    public override void Update()
    {
        base.Update();
        // Debug.Log("y: " + rb.linearVelocity.y);

        if (rb.linearVelocity.y < 0 && player.groundDetected != true)
            stateMachine.ChangeState(player.fallState);

        if (input.Player.Jump.WasPressedThisFrame())
            stateMachine.ChangeState(player.jumpState);
       
        if (input.Player.Attack.WasPressedThisFrame())
            stateMachine.ChangeState(player.basicAttackState);

        if (input.Player.Parry.WasPressedThisFrame())
            stateMachine.ChangeState(player.parryState);

        if (input.Player.RangeAttack.WasPressedThisFrame() && skillManager.swordThrow.CanUseSkill())
            stateMachine.ChangeState(player.swordThrowState);
    }
}
