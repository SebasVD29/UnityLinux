using UnityEngine;

public class Player_AnimationTriggers : Entity_AnimationTriggers
{
    private Player player;
    private Player_Combat playerCombat;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponentInParent<Player>();
    }

    private void ThrowSword() => player.skillManager.swordThrow.ThrowSword();
    //private void DashAttack() => player.skillManager.dash.DashAttack();
   
    //private void StealHealthParry() => playerCombat.PerfectParryAttack();
    //private void PerfectParry() => playerCombat.PerfectParryAttack();
}
