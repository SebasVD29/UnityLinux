using UnityEngine;

public class EnemyReaper_SpellCastState : EnemyState
{
    private Enemy_Reaper enemyReaper;
    public EnemyReaper_SpellCastState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }

    public override void Enter()
    {
        base.Enter();
        enemyReaper.MakeUntargetable(true);
        enemyReaper.SetVelocity(0, 0);
        enemyReaper.SetSpellCastPreformed(false);
        enemyReaper.SetSpellCastOnCooldown();
    }

    public override void Update()
    {
        base.Update();

        if (enemyReaper.spellCastPreformed)
            anim.SetBool("SpellCast_Performed", true);

        if (triggerCalled)
        {
            //if (enemyReaper.ShouldTeleport())
            //    stateMachine.ChangeState(enemyReaper.reaperTeleportState);
            //else
                stateMachine.ChangeState(enemyReaper.reaperBattleState);
        }

    }

    public override void Exit()
    {
        base.Exit();
        anim.SetBool("SpellCast_Performed", false);
        enemyReaper.MakeUntargetable(false);
    }
}
