using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class EnemyBoss_Combat : Entity_Combat
{
    [Header("Attack Range Overrides")]
    public float targetShortRadius = 2f;
    public float targetMediumRadius = 3f;
    public float targetLargeRadius = 3f;

    public void PerformAttackType(BossAttackType attackType)
    {
        float range = GetBossAttackRange(attackType);


        // Usa el mismo flujo base pero con un rango personalizado
        bool targetGotHit = false;
        foreach (var target in GetDetectedColliders(whatIsTarget, range))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();
            if (damageable == null) continue;

            AttackData attackData = stats.GetAttackData(basicAttackScale);
            Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

            float physicalDamage = attackData.phyiscalDamage;
            float elementalDamage = attackData.elementalDamage;
            ElementType element = attackData.element;

            targetGotHit = damageable.TakeDamage(physicalDamage, elementalDamage, transform, element);

            if (element != ElementType.None)
                statusHandler?.ApplyStatusEffect(element, attackData.effectData);

            if (targetGotHit)
            {
                RaisePhysicalDamageEvent(physicalDamage);
                vfx.UpdateOnHitColor(element);
                vfx.CreateOnHitVFX(target.transform, attackData.isCrit);
            }
        }
    }

    protected Collider2D[] GetDetectedColliders(LayerMask whatToDetect, float radius)
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, radius, whatToDetect);
    }

    public override void OnDrawGizmos()
    {
        
        base.OnDrawGizmos();

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(targetCheck.position, targetShortRadius);

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(targetCheck.position, targetMediumRadius);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(targetCheck.position, targetLargeRadius);
    }

    private float GetBossAttackRange(BossAttackType attackType)
    {
        switch (attackType)
        {
            case BossAttackType.Melee:
                return targetShortRadius;
          
            case BossAttackType.MagicMelee:
                return targetShortRadius;

            default:
                return targetCheckRadius;
        }
    }
}
