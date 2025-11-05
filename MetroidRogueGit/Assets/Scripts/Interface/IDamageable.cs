using UnityEngine;

public interface IDamageable 
{
    public bool TakeDamage(float damage, float elemental, Transform damageDealer, ElementType element);


}
