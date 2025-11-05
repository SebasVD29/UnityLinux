using UnityEngine;

public abstract class PlayerState : EntityState
{
    protected Player player;
    protected PlayerInputSet input;
    protected Player_SkillManager skillManager;

    protected float dashTimer;
    protected float skillDashTimer;


    public PlayerState(Player player, StateMachine stateMachine, string animBoolName) : base(stateMachine, animBoolName)
    {
        this.player = player;

        anim = player.anim;
        rb = player.rb;
        input = player.input;
        stats = player.stats;
        skillManager = player.skillManager;
        
    }
    public override void Enter()
    {
        base.Enter();
        
    }

    public override void Update()
    {
        base.Update();

        if (dashTimer > 0)
            dashTimer -= Time.deltaTime;

        anim.SetFloat("yVelocity", rb.linearVelocity.y);

        if (input.Player.Dash.WasPressedThisFrame() )
        {
            if (CanSkillDash())
            {
                skillManager.dash.SetSkillOnCooldown();
                stateMachine.ChangeState(player.skillDashState);
            }           
            else if (CanDash())
            {
                dashTimer = player.dashCooldown;
                stateMachine.ChangeState(player.dashState);
            }
        }

        
    }

    private bool CommonDashChecks()
    {

        if (player.wallDetected)
            return false;

        if (!player.groundDetected && !player.canAirDash)
            return false;

        return true;
    }

    private bool CanDash()
    {

        if (!CommonDashChecks())
            return false;

        if (stateMachine.currentState == player.dashState)
            return false;

        if (dashTimer > 0)
        {
            Debug.Log("On Cooldown dash");
            return false;
        }

        return true;
    }

    private bool CanSkillDash()
    {

        if (!CommonDashChecks())
            return false;

        if (stateMachine.currentState == player.skillDashState)
            return false;

        if (skillManager.dash.CanUseSkill() == false)
            return false;


        return true;
    }

    public bool CanDobleJump()
    {
        if (!player.hasJumped) // Evita doble salto si no se ha hecho el primero
            return false;

        if (stateMachine.currentState == player.doubleJumpState)
            return false;

        if (player.groundDetected)
            return false;

        if (!player.canDoubleJump)
            return false;

        return true;
       
    }
}
