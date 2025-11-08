using UnityEngine;

public class Enemy_Skeleton : Enemy , ICounterable
{
    public bool CanBeParry { get => canBeStunned; }
    public bool CanBePerfectParry { get => canBePerfect; }

    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "Idle");
        moveState = new Enemy_MoveState(this, stateMachine, "Move");
        attackState = new Enemy_AttacksState(this, stateMachine, "Attack");
        battleState = new Enemy_BattleState(this, stateMachine, "Battle");
        deadState = new Enemy_DeadState(this, stateMachine, "Idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "Stunned");

    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);
    
    }

    protected override void Update()
    {
        base.Update();

    }


    public void HandleCounter()
    {
        if(CanBeParry == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }

    public void HandlePerfectCounter()
    {
        if (CanBePerfectParry == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }
}
