using UnityEngine;

public class EnemyReaper_BattleState : Enemy_BattleState
{
    private Enemy_Reaper enemyReaper;
    private float nextDecisionTime;
    public EnemyReaper_BattleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
    }
    public override void Enter()
    {
        base.Enter();
        enemyReaper.MakeUntargetable(false);
        enemyReaper.UpdatePhase();

        if (!enemyReaper.HasPatternLoadedForPhase(enemyReaper.currentPhase))
        {
            enemyReaper.LoadAttackPatternForPhase(enemyReaper.currentPhase);
        }

        nextDecisionTime = Time.time + Random.Range(1.2f, 2.5f);
        stateTimer = enemyReaper.maxBattleIdleTime;
    }

    public override void Update()
    {
        HandleArenaMovement(); // 🔥 se mueve solo si está lejos

        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();
        enemyReaper.UpdatePhase();

        if (Time.time >= nextDecisionTime)
        {
            nextDecisionTime = Time.time + Random.Range(1.5f, 3f);
            ExecuteAttackPattern();
        }
    }
    private void HandleArenaMovement()
    {
        float distance = Mathf.Abs(enemyReaper.player.position.x - enemyReaper.transform.position.x);

        // Si está muy lejos, que camine hacia el jugador
        if (distance > 5f)
            enemyReaper.SetVelocity(enemyReaper.GetBattleMoveSpeed() * Mathf.Sign(enemyReaper.player.position.x - enemyReaper.transform.position.x),
                                    enemyReaper.rb.linearVelocity.y);
        else
            enemyReaper.SetVelocity(0, enemyReaper.rb.linearVelocity.y);
    }

    private void ExecuteAttackPattern()
    {
        var nextAttack = enemyReaper.PeekNextAttackFromPattern();

        // Si el ataque requiere posición específica, teletransportarse primero
        TeleportType requiredTp = enemyReaper.GetTeleportForAttack(nextAttack);
        bool requiresTeleport = requiredTp != TeleportType.Random;

        if (requiresTeleport && enemyReaper.CanTeleport())
        {
            enemyReaper.tpType = requiredTp;
            enemyReaper.SetTeleportOnCooldown();
            stateMachine.ChangeState(enemyReaper.reaperTeleportState);
            return;
        }

        // Si no requiere TP o ya se ha hecho, avanzamos el patrón
        nextAttack = enemyReaper.DequeueNextAttackFromPattern();

        Debug.Log("Patron " + nextAttack.type);
        // Guardás el delay del ataque actual para el siguiente ciclo
        nextDecisionTime = Time.time + nextAttack.postDelay;

        switch (nextAttack.type)
        {
            case BossAttackType.Melee:
                stateMachine.ChangeState(enemyReaper.reaperAttackState);
                break;

            case BossAttackType.MagicMelee:
                if (enemyReaper.tpType == TeleportType.Left || enemyReaper.tpType == TeleportType.Right)
                    stateMachine.ChangeState(enemyReaper.reaperMagicAttackState);
                break;

            case BossAttackType.Spell:
                if (enemyReaper.CanDoSpellCast())
                    stateMachine.ChangeState(enemyReaper.reaperSpellCastState);
                break;

            case BossAttackType.BulletHellWall:
                if (enemyReaper.tpType == TeleportType.Center)
                    stateMachine.ChangeState(enemyReaper.reaperMagicWallState);
                break;
        }
    }


}
