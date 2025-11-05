using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SceneManagement;

public class Player : Entity
{
    public static Player instance;
    public static event Action OnPlayerDeath;


    public UI ui { get; private set; }
    public PlayerInputSet input { get; private set; }
    public Player_SkillManager skillManager { get; private set; }
    public Player_Health health { get; private set; }
    public Player_VFX vfx { get; private set; }
    public Entity_StatusHandler statusHandler { get; private set; }
    public Player_Combat combat { get; private set; }
    public Player_Stats stats { get; private set; }
    public Inventory_Player inventory { get; private set; }

    #region State Variables
    public Player_IdleState idleState { get; private set; }
    public Player_MoveState moveState { get; private set; }
    public Player_JumpState jumpState { get; private set; }
    public Player_DoubleJumpState doubleJumpState { get; private set; }
    public Player_FallState fallState { get; private set; }
    public Player_WallSlideState wallSlideState { get; private set; }
    public Player_WallJumpState wallJumpState { get; private set; }
    public Player_DashState dashState { get; private set; }
    public Player_BasicAttackState basicAttackState { get; private set; }
    public Player_JumpAttackState jumpAttackState { get; private set; }
    public Player_ParryState parryState { get; private set; }
    public Player_DeadState deadState { get; private set; }
    public Player_SwordThrowState swordThrowState { get; private set; }
    public Player_SkillDashState skillDashState { get; private set; }
    #endregion

    [Header("Attack details")]
    public Vector2[] attackVelocity;
    public Vector2 jumpAttackVelocity;
    public float attackVelocityDuration = .1f;
    public float comboResetTime = 1;
    private Coroutine queuedAttackCo;
    [Space]

    [Header("Movement details")]
    public float moveSpeed;
    [Range(0, 1)]
    public float inAirMoveMultiplier = .7f; // Should be from 0 to 1;

    [Space]
    [Header("Movement Jump details")]
    public bool canDoubleJump = true;
    public bool hasJumped;

    public float jumpForce = 5;
    public float doubleJumpForce;

    [Space]
    [Header("Movement Dash details")]
    public bool canAirDash = true;
    public float dashDuration = 0.25f;
    public float dashSpeed = 20;
    public float dashCooldown = .30f;

    [Space]
    [Header("Movement WallJump details")]
    public Vector2 wallJumpForce;
    [Range(0, 1)]
    public float wallSlideSlowMultiplier = .7f;
   

    [Header("Move")]
    public Vector2 moveInput { get; private set; }
    public Vector2 mousePosition { get; private set; }
    public Vector2 stickDirection { get; private set; }

    protected override void Awake()
    {
      
        base.Awake();

        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);


        ui = FindAnyObjectByType<UI>();
        vfx = GetComponent<Player_VFX>();
        health = GetComponent<Player_Health>();
        skillManager = GetComponent<Player_SkillManager>();
        statusHandler = GetComponent<Entity_StatusHandler>();
        combat = GetComponent<Player_Combat>();
        stats = GetComponent<Player_Stats>();
        inventory = GetComponent<Inventory_Player>();

        input = InputManager.Instance.InputSet;

        idleState = new Player_IdleState(this, stateMachine, "Idle");
        moveState = new Player_MoveState(this, stateMachine, "Move");
        jumpState = new Player_JumpState(this, stateMachine, "JumpFall");
        doubleJumpState = new Player_DoubleJumpState(this, stateMachine, "JumpFall");
        fallState = new Player_FallState(this, stateMachine, "JumpFall");
        wallSlideState = new Player_WallSlideState(this, stateMachine, "WallSlide");
        wallJumpState = new Player_WallJumpState(this, stateMachine, "JumpFall");
        dashState = new Player_DashState(this, stateMachine, "Dash");
        skillDashState = new Player_SkillDashState(this, stateMachine, "Dash");
        basicAttackState = new Player_BasicAttackState(this, stateMachine, "BasicAttack");
        jumpAttackState = new Player_JumpAttackState(this, stateMachine, "JumpAttack");
        parryState = new Player_ParryState(this, stateMachine, "PlayerCouter");
        deadState = new Player_DeadState(this, stateMachine, "Dead");
        swordThrowState = new Player_SwordThrowState(this, stateMachine, "SwordThrow");

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
 
    public override void EntityDeath()
    {
        base.EntityDeath();
        OnPlayerDeath?.Invoke();
        stateMachine.ChangeState(deadState);
    }

    private void OnEnable()
    {
        //input.Player.Enable();
        //mouse
        input.Player.Mouse.performed += ctx => mousePosition = ctx.ReadValue<Vector2>();
        //Stick
        input.Player.AimStick.performed += ctx => stickDirection = ctx.ReadValue<Vector2>();

        input.Player.Movement.performed += ctx => moveInput = ctx.ReadValue<Vector2>();
        input.Player.Movement.canceled += ctx => moveInput = Vector2.zero;
    }


    public void EnterAttackStateWithDelay()
    {
        if (queuedAttackCo != null)
            StopCoroutine(queuedAttackCo);

        queuedAttackCo = StartCoroutine(EnterAttackStateWithDelayCo());
    }
    private IEnumerator EnterAttackStateWithDelayCo()
    {
        yield return new WaitForEndOfFrame();
        stateMachine.ChangeState(basicAttackState);
    }
    protected override IEnumerator SlowDownEntityCo(float duration, float slowMultiplier)
    {
        float originalMoveSpeed = moveSpeed;
        float originalJumpForce = jumpForce;
        float originalAnimSpeed = anim.speed;
        Vector2 originalWallJump = wallJumpForce;
        Vector2 originalJumpAttack = jumpAttackVelocity;
        Vector2[] originalAttackVelocity = attackVelocity;

        float speedMultiplier = 1 - slowMultiplier;

        moveSpeed = moveSpeed * speedMultiplier;
        jumpForce = jumpForce * speedMultiplier;
        anim.speed = anim.speed * speedMultiplier;
        wallJumpForce = wallJumpForce * speedMultiplier;
        jumpAttackVelocity = jumpAttackVelocity * speedMultiplier;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = attackVelocity[i] * speedMultiplier;
        }

        yield return new WaitForSeconds(duration);

        moveSpeed = originalMoveSpeed;
        jumpForce = originalJumpForce;
        anim.speed = originalAnimSpeed;
        wallJumpForce = originalWallJump;
        jumpAttackVelocity = originalJumpAttack;

        for (int i = 0; i < attackVelocity.Length; i++)
        {
            attackVelocity[i] = originalAttackVelocity[i];
        }
    }

    public void TeleportPlayer(Vector3 position) => transform.position = position;

    public void ResetPlayer()
    {
        // Reset estado de salud
        if (health != null)
            health.ResetHealth(); // Asegúrate que ResetHealth() restaura vida y isDead=false

        // Reset estados
        stateMachine.ChangeState(idleState);
        rb.simulated = true;
        //anim.Play("Idle");

    }
}
