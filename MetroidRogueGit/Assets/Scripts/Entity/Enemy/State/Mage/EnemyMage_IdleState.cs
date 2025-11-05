using UnityEngine;

public class EnemyMage_IdleState : Enemy_IdleState
{
    public EnemyMage_IdleState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }

    public override void Update()
    {
        stateTimer -= Time.deltaTime;
        UpdateAnimationParameters();

        if (enemy.PlayerDetectedAreaCollider())
            stateMachine.ChangeState(enemy.battleState);

    }

    
}
