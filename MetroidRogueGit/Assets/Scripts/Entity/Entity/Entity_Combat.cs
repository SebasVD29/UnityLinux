using System;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;

public class Entity_Combat : MonoBehaviour
{
    public event Action<float> OnDoingPhysicalDamage;
    protected Entity_VFX vfx;
    protected Entity_Stats stats;

    public DamageScaleData basicAttackScale;

    [Header("Target detection")]
    [SerializeField] protected Transform targetCheck;
    [SerializeField] protected float targetCheckRadius = 1;
    [SerializeField] protected LayerMask whatIsTarget;

    [Header("Status effect details")]
    [SerializeField] private float defaultDuration = 3;
    [SerializeField] private float chillSlowMultiplier = .2f;
    [SerializeField] private float electrifyChargeBuildUp = .4f;
    [Space]
    [SerializeField] private float fireScale = .8f;
    [SerializeField] private float electrifyScale = 2.5f;



    protected virtual void Awake()
    {
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();
    }

    public virtual void PerformAttack()
    {
        bool targetGotHit = false;

        foreach (var target in GetDetectedColliders(whatIsTarget))
        {
            IDamageable damageable = target.GetComponent<IDamageable>();

            if (damageable == null)
                continue; // skip target, go to next target

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
                OnDoingPhysicalDamage?.Invoke(physicalDamage);
                vfx.UpdateOnHitColor(element);
                vfx.CreateOnHitVFX(target.transform, attackData.isCrit);
                //sfx?.PlayAttackHit();
            }
        }

        //if (targetGotHit == false)
        //    sfx?.PlayAttackMiss();

    }

    public void PerformAttackOnTarget(Transform target, DamageScaleData damageScaleData = null)
    {
        bool targetGotHit = false;


        IDamageable damageable = target.GetComponent<IDamageable>();

        if (damageable == null)
            return; // skip target, go to next target

        DamageScaleData damageScale = damageScaleData == null ? basicAttackScale : damageScaleData;
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
            vfx.CreateOnHitVFX(target.transform, attackData.isCrit);
            //sfx?.PlayAttackHit();
        }


        //if (targetGotHit == false)
        //    //sfx?.PlayAttackMiss();
    }
    public void ApplyStatusEffect(Transform target, ElementType element, float scaleFactor = 1)
    {
        Entity_StatusHandler statusHandler = target.GetComponent<Entity_StatusHandler>();

        if (statusHandler == null)
            return;

        if (element == ElementType.Ice && statusHandler.CanBeApplied(ElementType.Ice))
            statusHandler.ApplyChillEffect(defaultDuration, chillSlowMultiplier);

        if (element == ElementType.Fire && statusHandler.CanBeApplied(ElementType.Fire))
        {
            scaleFactor = fireScale;
            float fireDamage = stats.offense.fireDamage.GetValue() * scaleFactor;
            statusHandler.ApplyBurnEffect(defaultDuration, fireDamage);
        }

        if (element == ElementType.Lightning && statusHandler.CanBeApplied(ElementType.Lightning))
        {
            scaleFactor = electrifyScale;
            float lightningDamage = stats.offense.lightningDamage.GetValue() * scaleFactor;
            statusHandler.ApplyShockEffect(defaultDuration, lightningDamage, electrifyChargeBuildUp);
        }

    }

    protected Collider2D[] GetDetectedColliders(LayerMask whatToDetect)
    {
        return Physics2D.OverlapCircleAll(targetCheck.position, targetCheckRadius, whatToDetect);
    }
    protected void RaisePhysicalDamageEvent(float damage)
    {
        OnDoingPhysicalDamage?.Invoke(damage);
    }

    public virtual void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(targetCheck.position, targetCheckRadius);
    }


}
