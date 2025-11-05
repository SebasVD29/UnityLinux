using UnityEngine;

public class Enemy_Health : Entity_Health
{
    private Enemy enemy => GetComponent<Enemy>();

    public override bool TakeDamage(float damage, float elemental, Transform damageDealer, ElementType element)
    {
       bool wasHit =  base.TakeDamage(damage, elemental, damageDealer, element);

        if(wasHit == false)
            return false;

        if (damageDealer.GetComponent<Player>() != null)
            enemy.TryEnterBattleState(damageDealer);

        return true;

    }

   

}
