using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Player_Health : Entity_Health
{
    private Player player;

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }
    protected override void Die()
    {
        base.Die();
        player.ui.OpenDeathScreenUI();
        player.inventory.ResetAllInventoryAndEquipment();
        
    }

    public void AmbientalDamage()
    {

    }

    public override void InstaKill(int parryCountToInstaKill, int originalParryCount)
    {
        

        if (parryCountToInstaKill > 0)
        {
            // daño proporcional según el total de parries
            float proportionalDamage = stats.GetTotalHealth() / (float)originalParryCount;
            ReduceHealth(proportionalDamage);
        }
        else
        {
            // último parry: matar directo
            ReduceHealth(9999999);
        }
    }
    public override void StealHealth(Player stealerEntity, Enemy loseEntity)
    {
        
        loseEntity.health.ReduceHealth(10);
        stealerEntity.health.IncreaseHealth(10);
    }
}
