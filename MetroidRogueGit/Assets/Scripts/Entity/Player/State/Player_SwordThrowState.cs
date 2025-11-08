using UnityEngine;

public class Player_SwordThrowState : PlayerState
{

    private Camera mainCamera;
    public Player_SwordThrowState(Player player, StateMachine stateMachine, string animBoolName) : base(player, stateMachine, animBoolName)
    {
    }
    public override void Enter()
    {
        base.Enter();

        skillManager.swordThrow.EnableDots(true);

        if (mainCamera != Camera.main)
            mainCamera = Camera.main;
    }

    public override void Update()
    {
        base.Update();

        Vector2 dirToMouse = DirectionToMouse();

        player.SetVelocity(0, rb.linearVelocity.y);
        player.HandleFlip(dirToMouse.x);
        skillManager.swordThrow.PredictTrajectory(dirToMouse);


        if (input.Player.Attack.WasPressedThisFrame())
        {
            anim.SetBool("SwordThrowPerformed", true);

            skillManager.swordThrow.EnableDots(false);
            skillManager.swordThrow.ConfirmTrajectory(dirToMouse);

            // skill manager create sword
        }

        if (input.Player.RangeAttack.WasReleasedThisFrame() || triggerCalled)
            stateMachine.ChangeState(player.idleState);
    }
    public override void Exit()
    {
        base.Exit();
        anim.SetBool("SwordThrowPerformed", false);
        skillManager.swordThrow.EnableDots(false);
    }
    private Vector2 DirectionToMouse()
    {
        Vector2 playerPosition = player.transform.position;

        // 🖱️ Si hay input de mouse → usar mouse
        if (player.stickDirection.sqrMagnitude < 0.01f)
        {
            Vector2 worldMousePosition = mainCamera.ScreenToWorldPoint(player.mousePosition);
            Vector2 direction = worldMousePosition - playerPosition;
            return direction.normalized;
        }
        // 🎮 Si hay input de stick → usar stick
        else
        {
            return player.stickDirection.normalized;
        }
    }

}
