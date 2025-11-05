using UnityEngine;

public class Enemy_Shooter : Enemy
{
    public EnemyShooter_BattleState shooterBattleState {  get;  set; }

    [Header("Archer Elf Specifics")]
    [SerializeField] private GameObject arrowPrefab;
    [SerializeField] private Transform arrowStartPoint;
    [SerializeField] private float arrowSpeed = 8;

    public bool CanBeParry { get => canBeStunned; }
    public bool CanBePerfectParry { get => canBePerfect; }

    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "Idle");
        moveState = new Enemy_MoveState(this, stateMachine, "Move");
        attackState = new Enemy_AttacksState(this, stateMachine, "Attack");
        deadState = new Enemy_DeadState(this, stateMachine, "Idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "Stunned");

        shooterBattleState = new EnemyShooter_BattleState(this, stateMachine, "Battle");
    
        battleState = shooterBattleState;
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

    }

    public override void SpecialAttack()
    {
        GameObject newArrow = Instantiate(arrowPrefab, arrowStartPoint.position, Quaternion.identity);
        newArrow.GetComponent<EnemyShooter_Bullet>().SetupArrow(arrowSpeed * facingDir, combat);
    }

    public void HandleCounter()
    {
        if (CanBeParry == false)
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
