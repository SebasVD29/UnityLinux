using UnityEngine;

public class EnemyMage_BattleState : Enemy_BattleState
{

    public EnemyMage_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        rb.linearVelocity = Vector2.zero;
        UpdateBattleTimer();

    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        if (enemy.PlayerDetectedAreaCollider())
        {
            UpdateTargetIfNeeded();
            UpdateBattleTimer();
        }

        if (BattleTimeIsOver())
        {
            stateMachine.ChangeState(enemy.idleState);
        }

        // Mira al jugador
        enemy.HandleFlip(MageDirectionToPlayer());

        // Ataca si está en rango y puede
        if (WithinAttackRange() && CanAttack() && enemy.PlayerDetectedAreaCollider())
        {
            lastTimeAttacked = Time.time;
            stateMachine.ChangeState(enemy.attackState);
        }
    }

    protected override void UpdateTargetIfNeeded()
    {
        if (enemy.PlayerDetectedAreaCollider() == false)
            return;

        Transform newTarget = enemy.PlayerDetectedAreaCollider().transform;

        if (newTarget != lastTarget)
        {
            lastTarget = newTarget;
            player = newTarget;
        }
    }

    private int MageDirectionToPlayer()
    {
        switch (DirectionToPlayer())
        {
            case 0: return 0;
            case 1: return -1;
            case -1: return 1;
            default : return 0;
        }

    }
    
}
