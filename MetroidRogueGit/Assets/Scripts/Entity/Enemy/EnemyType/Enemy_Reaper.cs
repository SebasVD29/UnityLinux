using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Enemy_Reaper : EnemyBoss
{
    public EnemyReaper_AttackState reaperAttackState { get; private set; }
    public EnemyReaper_BattleState reaperBattleState { get; private set; }
    public EnemyReaper_TeleportState reaperTeleportState { get; private set; }
    public EnemyReaper_MagicAttackState reaperMagicAttackState { get; private set; }
    public EnemyReaper_SpellCastState reaperSpellCastState { get; private set; }
  
    public EnemyReaper_DeathState reaperDeathState { get; private set; }


    [Header("Reaper specifics")]
    public float maxBattleIdleTime = 5;

    [Header("Reapert Teleport")]
    public TeleportType tpType;
    [SerializeField] private float offsetCenterY = -2.12f;
    [SerializeField] private float teleportCooldown = 6f;
    public float chanceToTeleport = .25f;
    private float lastTeleportTime = Mathf.NegativeInfinity;
    private float defaultTeleportChance;
    public bool teleporTrigger { get; private set; }

    [Header("Reaper Spellcast")]
    [SerializeField] private DamageScaleData spellDamageScale;
    [SerializeField] private GameObject spellCastPrefab;
    [SerializeField] private int amountToCast = 6;
    [SerializeField] private float spellCastRate = 1.2f;
    [SerializeField] private float spellCastStateCooldown = 10;
    [SerializeField] private Vector2 playerOffsetPrediction;
    private float lastTimeCastedSpells = float.NegativeInfinity;
    private bool canSpell = false;  
    public bool spellCastPreformed { get; private set; }
    private Player playerScript;

    [Header("Reaper Magic Tajo")]
    [SerializeField] private GameObject magicTajo;
    [SerializeField] private Transform tajoStartPoint;
    [SerializeField] private float tajoSpeed = 8;


    [Header("Reaper BulletHellPortal")]
    [SerializeField] private GameObject bulletHellPortalPrefab;


    protected override void Awake()
    {
        base.Awake();
        idleState = new Enemy_IdleState(this, stateMachine, "Idle");
        moveState = new Enemy_MoveState(this, stateMachine, "Move");
        stunnedState = new Enemy_StunnedState(this, stateMachine, "Stunned");

        reaperBattleState = new EnemyReaper_BattleState(this, stateMachine, "Battle");
        reaperAttackState = new EnemyReaper_AttackState(this, stateMachine, "Attack");
        reaperTeleportState = new EnemyReaper_TeleportState(this, stateMachine, "Teleport");
        reaperMagicAttackState = new EnemyReaper_MagicAttackState(this, stateMachine, "MagicAttack");
        reaperSpellCastState = new EnemyReaper_SpellCastState(this, stateMachine, "SpellCast");
        reaperDeathState = new EnemyReaper_DeathState(this, stateMachine, "Death");

        battleState = reaperBattleState;
       
    }

    protected override void Start()
    {
        base.Start();

        arenaBounds.transform.parent = null;
        defaultTeleportChance = chanceToTeleport;

    }
    public override void EntityDeath()
    {
        stateMachine.ChangeState(reaperDeathState);
    }

    public override void ApplyPhaseModifiers()
    {
        switch (currentPhase)
        {
            case BossPhase.Phase1:
                battleMoveSpeed = 3;
                chanceToTeleport = 0.15f;
                attackCooldown = 1.2f;
                break;

            case BossPhase.Phase2:
                battleMoveSpeed = 3.5f;
                chanceToTeleport = 0.25f;
                attackCooldown = 0.9f;
                amountToCast = 6;
                canSpell = true;
                break;

            case BossPhase.Phase3:
                battleMoveSpeed = 4f;
                chanceToTeleport = 0.3f;
                attackCooldown = 0.6f;
                amountToCast = 10;
                break;
        }
    }
    public bool ShouldTeleport()
    {
        if (Random.value < chanceToTeleport)
        {
            chanceToTeleport = defaultTeleportChance;
            return true;
        }

        chanceToTeleport = chanceToTeleport + .05f;
        return false;
    }

    public TeleportType GetTeleportForAttack(AttackTiming attack)
    {
        switch (attack.type)
        {
            case BossAttackType.MagicMelee:
                return Random.value < 0.5f ? TeleportType.Left : TeleportType.Right;

            case BossAttackType.BulletHell:
                return TeleportType.Center;

            case BossAttackType.Spell:
                return Random.value < 0.5f ? TeleportType.Left : TeleportType.Right;

            case BossAttackType.Melee:
                return TeleportType.Random;

            default:
                return TeleportType.Random;
        }
    }
    public void SetTeleportTrigger(bool triggerStatus) => teleporTrigger = triggerStatus;

    public Vector3 FindTeleportPoint(TeleportType tpType = TeleportType.Random)
    {
        float bossWithColliderHalf = col.bounds.size.x / 2;
        Vector2 raycastPoint;
        float targetX;

        switch (tpType)
        {
            case TeleportType.Left:
                targetX = arenaBounds.bounds.min.x + bossWithColliderHalf + 1f; // margen a la izquierda
                break;

            case TeleportType.Right:
                targetX = arenaBounds.bounds.max.x - bossWithColliderHalf - 1f; // margen a la derecha
                break;

            case TeleportType.Center:
                targetX = (arenaBounds.bounds.min.x + arenaBounds.bounds.max.x) / 2f; // punto medio del bounds
                break;

            default: // TeleportType.Random
                targetX = Random.Range(arenaBounds.bounds.min.x + bossWithColliderHalf,
                                       arenaBounds.bounds.max.x - bossWithColliderHalf);
                break;
        }

        raycastPoint = new Vector2(targetX, arenaBounds.bounds.max.y);

        // Hacer raycast hacia abajo para encontrar el suelo
        RaycastHit2D hit = Physics2D.Raycast(raycastPoint, Vector2.down, Mathf.Infinity, whatIsGround);
        if (hit.collider != null)
            return hit.point - new Vector2(0, offsetCenterY);

        return transform.position;
    }
    public override void SpecialAttack()
    {
        if (currentPhase == BossPhase.Phase3 && tpType == TeleportType.Center)
        {
            StartCoroutine(CastBulletHellPortalCo());
        }
        else 
        {
            StartCoroutine(CastSpellCo());
        }
    }
    public bool CanDoSpellCast()
    {
        if(!canSpell)
            return false;

        if(Time.time < lastTimeCastedSpells + spellCastStateCooldown)
            return false;

        return true;
        
    } 
    public void CreateMagicTajo()
    {
        GameObject newArrow = Instantiate(magicTajo, tajoStartPoint.position, Quaternion.identity);
        newArrow.GetComponent<EnemyReaper_MagicTajo>().SetupMagicTajo(tajoSpeed * facingDir, combat);
    }
    public void SetSpellCastPreformed(bool spellCastStatus) => spellCastPreformed = spellCastStatus;
    public void SetSpellCastOnCooldown() => lastTimeCastedSpells = Time.time;
    public bool CanTeleport() => Time.time > lastTeleportTime + teleportCooldown;
    public void SetTeleportOnCooldown() => lastTeleportTime = Time.time;
    private IEnumerator CastSpellCo()
    {
        if (playerScript == null)
            playerScript = player.GetComponent<Player>();

        for (int i = 0; i < amountToCast; i++)
        {
            bool playerMoving = playerScript.rb.linearVelocity.magnitude > 0;

            float xOffset = playerMoving ? playerOffsetPrediction.x * playerScript.facingDir : 0;
            Vector3 spellPosition = player.transform.position + new Vector3(xOffset, playerOffsetPrediction.y);

            EnemyReaper_Spell spell
                = Instantiate(spellCastPrefab, spellPosition, Quaternion.identity).GetComponent<EnemyReaper_Spell>();

            spell.SetupSpell(combat, spellDamageScale);

            yield return new WaitForSeconds(spellCastRate);
        }

        SetSpellCastPreformed(true);
    }
    private IEnumerator CastBulletHellPortalCo()
    {
        float centerX = (arenaBounds.bounds.min.x + arenaBounds.bounds.max.x) / 2f;
        float groundY = (arenaBounds.bounds.min.y + arenaBounds.bounds.max.y) / 2f;
        float verticalOffset = 2.36f;

        Vector3 spawnPos = new Vector3(centerX, groundY + verticalOffset, 0f);

        // Crear el portal en el centro
        GameObject portal = Instantiate(bulletHellPortalPrefab, spawnPos, Quaternion.identity);
        EnemyReaper_SpellBulletHell portalScript = portal.GetComponent<EnemyReaper_SpellBulletHell>();
        portalScript.SetUpPortal(combat);

        // Destruir el portal cuando haya terminado su patrón de ataque
        StartCoroutine(DestroyPortalAfter(portal, portalScript.DuracionTotal()));

        yield return null;
    }

    private IEnumerator DestroyPortalAfter(GameObject portal, float duration)
    {
        yield return new WaitForSeconds(duration);
        Destroy(portal);
        SetSpellCastPreformed(true);
    }

}

