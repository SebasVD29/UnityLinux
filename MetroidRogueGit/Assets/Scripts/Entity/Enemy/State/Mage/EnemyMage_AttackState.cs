using UnityEngine;

public class EnemyMage_AttackState : Enemy_AttacksState
{
    private float attackStartTime;
    public EnemyMage_AttackState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();
        attackStartTime = Time.time;
        //enemy.SetVelocity(0, 0); // detener movimiento si existiera
    }
    public override void Update()
    {
        base.Update();

        
    }
}
