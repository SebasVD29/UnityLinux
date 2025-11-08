using UnityEngine;

public class EnemyReaper_TeleportState : EnemyState
{
    private Enemy_Reaper enemyReaper;
    private bool hasTeleported;
    public EnemyReaper_TeleportState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }
    public override void Enter()
    {
        base.Enter();
        hasTeleported = false;
        enemyReaper.MakeUntargetable(true);

        // Saber qué ataque viene después del teleport
        var upcomingAttack = enemyReaper.PeekNextAttackFromPattern();

        // Elegir teleport según el ataque
        enemyReaper.tpType = enemyReaper.GetTeleportForAttack(upcomingAttack);
    }

    public override void Update()
    {
        base.Update();

        if (enemyReaper.teleporTrigger && !hasTeleported)
        {
            hasTeleported = true;
            enemyReaper.transform.position = enemyReaper.FindTeleportPoint(enemyReaper.tpType);
            enemyReaper.SetTeleportTrigger(false);

            enemyReaper.FlipTowardsPlayer();
        }

        if (triggerCalled)
        {
            // Elegimos el siguiente estado según la fase y el tipo de teleport
            HandlePostTeleportAction();
        }
    }

    public override void Exit()
    {
        base.Exit();
        enemyReaper.MakeUntargetable(false);
        hasTeleported = false;
    }

    private void HandlePostTeleportAction()
    {
        var nextAttack = enemyReaper.DequeueNextAttackFromPattern();

        // Elegir el siguiente estado según el tipo de ataque
        switch (nextAttack.type)
        {
            case BossAttackType.Melee:
                stateMachine.ChangeState(enemyReaper.reaperAttackState);
                break;

            case BossAttackType.MagicMelee:
                stateMachine.ChangeState(enemyReaper.reaperMagicAttackState);
                break;

            case BossAttackType.Spell:
                stateMachine.ChangeState(enemyReaper.reaperSpellCastState);
                break;

            case BossAttackType.BulletHell:
                stateMachine.ChangeState(enemyReaper.reaperSpellCastState);
                break;

            default:
                stateMachine.ChangeState(enemyReaper.reaperBattleState); // fallback
                break;
        }
    }


}
