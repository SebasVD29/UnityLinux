using System.Collections;
using UnityEditor;
using UnityEngine;

public class Entity_StatusHandler : MonoBehaviour
{
    private Entity entity;
    private Entity_VFX vfx;
    private Entity_Stats stats;
    private Entity_Health health;
    private ElementType currentEffect = ElementType.None;

    [Header("Electricfy effect")]
    [SerializeField] private GameObject lightningStrikeVfx;
    [SerializeField] private float currentCharge;
    [SerializeField] private float maximumCharge;
    private Coroutine shockCo;

   

    private void Awake()
    {
        entity = GetComponent<Entity>();
        vfx = GetComponent<Entity_VFX>();
        stats = GetComponent<Entity_Stats>();
        health = GetComponent<Entity_Health>();
    }


    public void ApplyStatusEffect(ElementType element, ElementalEffectData effectData)
    {
        if (element == ElementType.Ice && CanBeApplied(ElementType.Ice))
            ApplyChillEffect(effectData.chillDuration, effectData.chillSlowMultiplier);

        if (element == ElementType.Fire && CanBeApplied(ElementType.Fire))
            ApplyBurnEffect(effectData.burnDuratoin, effectData.totalBurnDamage);

        if (element == ElementType.Lightning && CanBeApplied(ElementType.Lightning))
            ApplyShockEffect(effectData.shockDuration, effectData.shockDamage, effectData.shockCharge);
    }
    public void ApplyChillEffect(float duration, float slowMultiplier)
    {
        float iceResistence = stats.GetElementalResistance(ElementType.Ice);
        float reduceDuration = duration * (1 - iceResistence);

        StartCoroutine(ChillEffectCo(reduceDuration, slowMultiplier));
    }
    public void ApplyBurnEffect(float duration, float fireDamage)
    {
        float fireResistence = stats.GetElementalResistance(ElementType.Fire);
        float reduceFireDamage = fireDamage * (1 - fireResistence);

        StartCoroutine(BurnEffectCo(duration, reduceFireDamage));
    }
    public void ApplyShockEffect(float duration, float electrifyDamage, float charge)
    {
        float lightningResistence = stats.GetElementalResistance(ElementType.Lightning);
        float reduceLightningCharge = charge * (1 - lightningResistence);

        currentCharge = currentCharge + reduceLightningCharge;

        if (currentCharge >= maximumCharge)
        {
            DoLightningStrike(electrifyDamage);
            StopShockEffect();
            return;
        }


        if(shockCo != null)
            StopCoroutine(shockCo);

        shockCo =  StartCoroutine(ShockEffectCo(duration));
    }

    void StopShockEffect()
    {
        currentEffect = ElementType.None;
        currentCharge = 0;
        vfx.StopAllVfx();
    }

    void DoLightningStrike(float damage)
    {
        Instantiate(lightningStrikeVfx, transform.position, Quaternion.identity);
        health.ReduceHealth(damage);
    }
    public bool CanBeApplied(ElementType element)
    {
        if(element == ElementType.Lightning && currentEffect == ElementType.Lightning)
            return true;


        return currentEffect == ElementType.None;
    }

    private IEnumerator ChillEffectCo(float duration, float multiplier)
    {
        entity.SlowDownEntity(duration, multiplier);

        currentEffect = ElementType.Ice;
        vfx.PlayOnStatusVfx(duration, ElementType.Ice);

        yield return new WaitForSeconds(duration);

        currentEffect = ElementType.None;
    }

    private IEnumerator BurnEffectCo(float duration, float totalDamage)
    {
        currentEffect = ElementType.Fire;
        vfx.PlayOnStatusVfx(duration, ElementType.Fire);

        int tickerPerSecond = 2;
        int tickCount = Mathf.RoundToInt( tickerPerSecond * duration);

        float damagePerTick = totalDamage / tickCount;
        float tickInterval = 1f / tickerPerSecond;

        for (int i = 0; i<tickCount; i++)
        {
            health.ReduceHealth(damagePerTick);
            yield return new WaitForSeconds(tickInterval);
        }


        currentEffect = ElementType.None;
    }
    private IEnumerator ShockEffectCo(float duration)
    {
        currentEffect = ElementType.Lightning;
        vfx.PlayOnStatusVfx(duration, ElementType.Lightning);
        yield return new WaitForSeconds(duration);

        StopShockEffect();

        currentEffect = ElementType.None;
    }
}
