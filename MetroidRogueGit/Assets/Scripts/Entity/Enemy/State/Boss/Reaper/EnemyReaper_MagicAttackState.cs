using UnityEngine;

public class EnemyReaper_MagicAttackState : EnemyState
{
    private Enemy_Reaper enemyReaper;
 
    public EnemyReaper_MagicAttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }
    public override void Enter()
    {
        base.Enter();
        SyncAttackSpeed();
 
       
    }

    public override void Update()
    {
        base.Update();

        if (triggerCalled )
        {

            //if (enemyReaper.ShouldTeleport())
            //    stateMachine.ChangeState(enemyReaper.reaperTeleportState);
            //else
                stateMachine.ChangeState(enemyReaper.reaperBattleState);
        }
    }
}
