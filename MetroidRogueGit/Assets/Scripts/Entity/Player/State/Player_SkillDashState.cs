using UnityEngine;

public class Player_SkillDashState : Player_DashState
{
   
    public Player_SkillDashState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        skillManager.dash.Dash();

    }

    public override void Update()
    {
        base.Update();
      
    }
}
