using System.Collections;
using UnityEngine;

public class Enemy : Entity
{
    public Entity_Stats stats { get; private set; }
    public Enemy_Health health { get; private set; }
    public Entity_Combat combat { get; private set; }
    public Entity_VFX vfx { get; private set; }


    public Enemy_IdleState idleState;
    public Enemy_MoveState moveState;
    public Enemy_AttacksState attackState;
    public Enemy_BattleState battleState;
    public Enemy_DeadState deadState;
    public Enemy_StunnedState stunnedState;



    [Header("Battle")]
    public float battleMoveSpeed = 3;
    public float attackDistance = 2;
    public float attackCooldown = .5f;
    public bool canChasePlayer = true;
    [Space]
    public float battleTimeDuration = 5;
    public float minRetreatDistance = 1;
    public Vector2 retreatVelocity;
    
 

    [Header("Movement")]
    public float idleTime = 2;
    public float moveSpeed = 1.4f;
    [Range(0f, 2f)]
    public float moveAnimSpeedMultiplier = 1;

    [Header("PlayerDetection")]
    [SerializeField] public LayerMask whatIsPlayer;
    [SerializeField] public Transform playerCheck;
    [SerializeField] public float playerCheckDistance = 10;
    public Transform player { get; private set; }
    public float activeSlowMultiplier { get; private set; } = 1;

    [Header("Stunned Details")]
    public float stunnedDuration = 1;
    public float stunnedPerfectDuration = 1;
    public Vector2 stunnedVelocity = new Vector2(7,7);
    protected bool canBeStunned;

    [Header("Parry Details")]
    protected bool canBePerfect;
    public int parryCountToInstaKill = 4;
    [SerializeField] private int parriesNeededPerLevel = 4;
    private int originalParryCountToInstaKill;
    private int parryProgress;

    protected override void Awake()
    {
        base.Awake();
        health = GetComponent<Enemy_Health>();
        stats = GetComponent<Entity_Stats>();
        combat = GetComponent<Entity_Combat>();
        originalParryCountToInstaKill = parryCountToInstaKill;
    }
    public override void EntityDeath()
    {
        base.EntityDeath();

        stateMachine.ChangeState(deadState);
    }
    public virtual void SpecialAttack()
    {

    }
    protected override IEnumerator SlowDownEntityCo(float duration, float multiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalBattleSpeed = battleMoveSpeed;
        float originalAnimSpeed = anim.speed;

        float speedMultiplier = 1 - multiplier;

        moveSpeed = moveSpeed * speedMultiplier;
        battleMoveSpeed = battleMoveSpeed * speedMultiplier;
        anim.speed = anim.speed * speedMultiplier;

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        battleMoveSpeed = originalBattleSpeed;
        anim.speed = originalAnimSpeed;

    }

    public void TryEnterBattleState(Transform player)
    {

        if (stateMachine.currentState == battleState || stateMachine.currentState == attackState)
            return;

        this.player = player;
        stateMachine.ChangeState(battleState);
    }

    public void FlipTowardsPlayer()
    {
        if (player == null) return;

        float directionToPlayer = Mathf.Sign(player.position.x - transform.position.x);

        if (directionToPlayer != facingDir)
            Flip();
    }
    public void MakeUntargetable(bool canBeTargeted)
    {
        if (canBeTargeted == true)
        {
            gameObject.layer = LayerMask.NameToLayer("Untargetable");
        }
        else
        {
            gameObject.layer = LayerMask.NameToLayer("Enemy");
        }
    }
    public Transform GetPlayerReference()
    {
        if (player == null)
            player = PlayerDetected().transform;


        return player;
    }
    public void EnableCounter(bool enable)
    {
        canBeStunned = enable; 
    }

    public void EnablePerfectCounter(bool Perfect)
    {
        canBePerfect = Perfect;
    }

    protected override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * playerCheckDistance), playerCheck.position.y));

        Gizmos.color = Color.blue;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * attackDistance), playerCheck.position.y));

        Gizmos.color = Color.green;
        Gizmos.DrawLine(playerCheck.position, new Vector3(playerCheck.position.x + (facingDir * minRetreatDistance), playerCheck.position.y));

    }

    public virtual RaycastHit2D PlayerDetected()
    {
        RaycastHit2D hit = Physics2D.Raycast(playerCheck.position, Vector2.right * facingDir, playerCheckDistance, whatIsPlayer | whatIsGround);

        if (hit.collider == null || hit.collider.gameObject.layer != LayerMask.NameToLayer("Player"))
        {
            return default;
        }

        return hit;
    }
    public Collider2D PlayerDetectedAreaCollider()
    {
        return Physics2D.OverlapCircle(playerCheck.position, attackDistance, whatIsPlayer);
    }

    void HandlePlayerDeath()
    {
        stateMachine.ChangeState(idleState);
    }

    private void OnEnable()
    {
        Player.OnPlayerDeath += HandlePlayerDeath;

    }

    private void OnDisable()
    {
        Player.OnPlayerDeath -= HandlePlayerDeath;
    }



    public void ReduceParryCounts(bool parry)
    {
        if (!parry) return;

        if (parriesNeededPerLevel <= 0)
        {
            parryCountToInstaKill--;
            health.InstaKill(parryCountToInstaKill, originalParryCountToInstaKill);
            return;
        }

        parryProgress++;

        if (parryProgress >= parriesNeededPerLevel)
        {
            parryProgress = 0;
            parryCountToInstaKill--;

            // Cada vez que baja 1 nivel, ejecuta el cálculo de daño
            health.InstaKill(parryCountToInstaKill, originalParryCountToInstaKill);
        }
    }

    public void StealHealthToPlayer(Player player)
    {
        if (player == null)
            return;

        health.StealHealth(player, this);
    }

    public float GetBattleMoveSpeed() => battleMoveSpeed * activeSlowMultiplier;
    public float GetMoveSpeed() => moveSpeed * activeSlowMultiplier;

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            SetVelocity(0, rb.linearVelocity.y);
        }
    }

}
