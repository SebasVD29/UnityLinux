using System.Linq;
using UnityEngine;

public class EnemyReaper_BattleState : Enemy_BattleState
{
    private Enemy_Reaper enemyReaper;
    
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

        stateTimer = enemyReaper.maxBattleIdleTime;
    }

    public override void Update()
    {
        HandleArenaMovement(); 

        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();
        enemyReaper.UpdatePhase();

        if (Time.time >= enemyReaper.nextDecisionTime)
        {
            
            ExecuteAttackPattern();
        }
    }
    private void HandleArenaMovement()
    {
        float distance = Mathf.Abs(enemyReaper.player.position.x - enemyReaper.transform.position.x);
        float moveSpeed = enemyReaper.GetBattleMoveSpeed();

        if (distance > 9f)
        {
            // Demasiado lejos
            enemyReaper.SetVelocity(moveSpeed * Mathf.Sign(enemyReaper.player.position.x - enemyReaper.transform.position.x),
                                    enemyReaper.rb.linearVelocity.y);
        }
        else if (distance > 5.7f) //6 - 7
        {
            // En rango medio
            enemyReaper.SetVelocity((moveSpeed * 0.5f) * Mathf.Sign(enemyReaper.player.position.x - enemyReaper.transform.position.x),
                                    enemyReaper.rb.linearVelocity.y);
        }
        else
        {
            if (ShouldRetreat())
            {
                ShortRetreat();
            }
        }
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

   
        // Guardás el delay del ataque actual para el siguiente ciclo
        enemyReaper.nextDecisionTime = Time.time + 1f + nextAttack.postDelay;

        switch (nextAttack.type)
        {
            case BossAttackType.Melee:
                if (WithinAttackRange())
                {
                    stateMachine.ChangeState(enemyReaper.reaperAttackState);
                }
                else
                {
                    enemyReaper.nextDecisionTime = Time.time + 0.5f; // 🔹 Pequeño cooldown antes de reintentar
                }
                break;

            case BossAttackType.MagicMelee:
                if (enemyReaper.tpType == TeleportType.Left || enemyReaper.tpType == TeleportType.Right)
                    stateMachine.ChangeState(enemyReaper.reaperMagicAttackState);
                break;

            case BossAttackType.Spell:
                if (enemyReaper.CanDoSpellCast() && (enemyReaper.tpType == TeleportType.Left || enemyReaper.tpType == TeleportType.Right))
                    stateMachine.ChangeState(enemyReaper.reaperSpellCastState);
                break;

            case BossAttackType.BulletHell:
                if (enemyReaper.CanDoSpellCast() && enemyReaper.tpType == TeleportType.Center)
                    stateMachine.ChangeState(enemyReaper.reaperSpellCastState);
                break;
        }
    }





}
