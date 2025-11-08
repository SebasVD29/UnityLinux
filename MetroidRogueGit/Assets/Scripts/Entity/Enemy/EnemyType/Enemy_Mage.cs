using UnityEngine;

public class Enemy_Mage : Enemy
{
    public EnemyMage_IdleState mageIdleState { get; set; }
    public EnemyMage_AttackState mageAttackState { get; set; }
    public EnemyMage_BattleState mageBattleState { get; set; }


    [Header("Fire Ball Specifics")]
    [SerializeField] private GameObject fireBallPrefab;
    [SerializeField] private Transform fireBallStartPoint; 
    private Vector2 lastKnownPlayerPosition;


    protected override void Awake()
    {
        base.Awake();
       
        deadState = new Enemy_DeadState(this, stateMachine, "Idle");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "Stunned");

        mageIdleState = new EnemyMage_IdleState(this, stateMachine, "Idle");
        mageAttackState = new EnemyMage_AttackState(this, stateMachine, "Attack");
        mageBattleState = new EnemyMage_BattleState(this, stateMachine, "Battle");

        idleState = mageIdleState;
        attackState = mageAttackState;
        battleState = mageBattleState;
    }

    protected override void Start()
    {
        base.Start();

        stateMachine.Initialize(idleState);

    }

    public override void SpecialAttack() //esto se llama como un animation trigger
    {
        if(PlayerDetectedAreaCollider())
            lastKnownPlayerPosition = PlayerDetectedAreaCollider().transform.position;

        GameObject newFireball = Instantiate(fireBallPrefab, fireBallStartPoint.position, Quaternion.identity);
        newFireball.GetComponent<EnemyMage_Fireball>().SetupFireBall(lastKnownPlayerPosition, combat);
    }

    protected override void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(playerCheck.position, attackDistance);
    }

  

}
