using UnityEngine;

public class Player_ParryState : PlayerState
{
    private Player_Combat playerCombat;
    private bool parrySomebody;
    public Player_ParryState(Player player, StateMachine stateMachine, string _animBoolName) : base(player, stateMachine, _animBoolName)
    {
        playerCombat = player.GetComponent<Player_Combat>();
    }
    public override void Enter()
    {
        base.Enter();

        stateTimer = playerCombat.GetParryRecovery();
        parrySomebody = playerCombat.ParryAttack(out bool isPerfect);


        //Debug.Log(isPerfect);
        if (isPerfect == true)
            anim.SetBool("PerfectParry", parrySomebody);
        else
            anim.SetBool("Parry", parrySomebody);




    }
    public override void Update()
    {
        base.Update();
        player.SetVelocity(0, rb.linearVelocity.y);

        if(triggerCalled)
            stateMachine.ChangeState(player.idleState);

        if (stateTimer < 0 && parrySomebody == false)
            stateMachine.ChangeState(player.idleState);
    }
}
