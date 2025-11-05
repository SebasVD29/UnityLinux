using UnityEngine;
using UnityEngine.Rendering;

public class Entity_Stats : MonoBehaviour
{
    public Stat_SetUpSO defaultSetup;

    [Header("Stats Base")]
    //public Stat maxHealth;
    public Stat_ResourceGroup resources;
    public Stat_MajorGroup major;
    public Stat_OffenseGroup offense;
    public Stat_DefenseGroup defense;

    protected virtual void Awake()
    {

    }
    public AttackData GetAttackData(DamageScaleData scaleData)
    {
        return new AttackData(this, scaleData);
    }


    public float GetElementalResistance(ElementType element)
    {
        float baseResistance = 0;
        float bonusResistance = major.intelligence.GetValue() * .5f; // Bonus resistance from intelligence: +0.5% per INT

        switch (element)
        {
            case ElementType.Fire:
                baseResistance = defense.fireRes.GetValue();
                break;
            case ElementType.Ice:
                baseResistance = defense.iceRes.GetValue();
                break;
            case ElementType.Lightning:
                baseResistance = defense.lightningRes.GetValue();
                break;
        }

        float resistance = baseResistance + bonusResistance;
        float resistanceCap = 80f; // Resistance will be capped at 75%;
        float finalResistance = Mathf.Clamp(resistance, 0, resistanceCap) / 100; // Convert value into 0 to 1 multiplier

        return finalResistance;
    }

    public float GetElementalDamage(out ElementType element, float scaleFactor = 1)
    {
        float fireDamage = offense.fireDamage.GetValue();
        float iceDamage = offense.iceDamage.GetValue();
        float lightningDamage = offense.lightningDamage.GetValue();

        float bonusElementalDamage = major.intelligence.GetValue();

        float highestDamage = fireDamage;
        element = ElementType.Fire;


        if (iceDamage > highestDamage)
        {
            highestDamage = iceDamage;
            element = ElementType.Ice;
        }

        if (lightningDamage > highestDamage)
        {
            highestDamage = lightningDamage;
            element = ElementType.Lightning;
        }

        if (highestDamage <= 0)
        {
            element = ElementType.None;
            return 0;
        }

        float bonusFire = (element == ElementType.Fire) ? 0 : fireDamage * .5f;
        float bonusIce = (element == ElementType.Ice) ? 0 : iceDamage * .5f;
        float bonusLigthning = (element == ElementType.Lightning) ? 0 : lightningDamage * .5f;

        float weakerElementsDamage = bonusFire + bonusIce + bonusLigthning;
        float finalDamage = highestDamage + weakerElementsDamage + bonusElementalDamage;

        return finalDamage * scaleFactor;
    }

    public float GetArmorMitigation(float armorReduction)
    {
        float totalArmor = GetBaseArmor();

        float reductionMutliplier = Mathf.Clamp(1 - armorReduction, 0, 1);
        float effectiveArmor = totalArmor * reductionMutliplier;

        float mitigation = effectiveArmor / (effectiveArmor + 100);
        float mitigationCap = .85f; // Max mitigation will be capped at 85%

        float finalMitigation = Mathf.Clamp(mitigation, 0, mitigationCap);

        return finalMitigation;
    }

    // Bonus armor from Vitality: +1 per VIT 
    public float GetBaseArmor() => defense.armor.GetValue() + major.vitality.GetValue();
    public float GetArmorReduction()
    {
        float finalReduction = offense.armorReduction.GetValue() / 100;
 
        return finalReduction;
    }
    public float GetPhyiscalDamage(out bool isCrit, float scaleFactor = 1)
    {
        float baseDamage = GetBaseDamage();
        float critChance = GetCritChance();
        float critPower = GetCritPower() / 100; // Total crit power as multiplier ( e.g 150 / 100 = 1.5f - multiplier)

        isCrit = Random.Range(0, 100) < critChance;
        float finalDamage = isCrit ? baseDamage * critPower : baseDamage;

        return finalDamage * scaleFactor;
    }

    // Bonus damage from Strength: +1 per STR
    public float GetBaseDamage() => offense.damage.GetValue() + major.strength.GetValue();
    //  Bonus crit chance from Agility: +0.3% per AGI 
    public float GetCritChance() => offense.critChance.GetValue() + (major.agility.GetValue() * .3f);
    // Bonus crit chance from Strength: +0.5% per STR 
    public float GetCritPower() => offense.critPower.GetValue() + (major.strength.GetValue() * .5f);

    public float GetBaseHealth() => resources.baseHealth.GetValue();
    
    public float GetTotalHealth()
    {
        float baseHp = resources.baseHealth.GetValue();
        float bonusHp = major.vitality.GetValue() * 5;

        float finalTotalHealth = baseHp + bonusHp;

        return finalTotalHealth;
    }

    public float GetEvasion()
    {
        float baseEvasion = defense.evasion.GetValue();
        float bonusEvasion = major.agility.GetValue() * .5f; // Bonus evasion from Agility: +0.5% per AGI 

        float totalEvasion = baseEvasion + bonusEvasion;
        float evasionCap = 85f; // Max evasion will be capped at 85%

        float finalEvasion = Mathf.Clamp(totalEvasion, 0, evasionCap);

        return finalEvasion;
    }

    public Stat GetStatByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return resources.baseHealth;
            case StatType.HealthRegen: return resources.healthRegen;
            case StatType.Strength: return major.strength;
            case StatType.Agility: return major.agility;
            case StatType.Intelligence: return major.intelligence;
            case StatType.Vitality: return major.vitality;
            case StatType.AttackSpeed: return offense.attackSpeed;
            case StatType.Damage: return offense.damage;
            case StatType.CritChance: return offense.critChance;
            case StatType.CritPower: return offense.critPower;
            case StatType.ArmorReduction: return offense.armorReduction;
            case StatType.FireDamage: return offense.fireDamage;
            case StatType.IceDamage: return offense.iceDamage;
            case StatType.LightningDamage: return offense.lightningDamage;
            case StatType.Armor: return defense.armor;
            case StatType.Evasion: return defense.evasion;
            case StatType.IceResistance: return defense.iceRes;
            case StatType.FireResistance: return defense.fireRes;
            case StatType.LightningResistance: return defense.lightningRes;

            default:
                Debug.LogWarning($"StatType {type} not implemented yet.");
                return null;
        }
    }


    [ContextMenu("Update Default Stat Setup")]
    public void ApplyDefaultStatSetup()
    {
        if (defaultSetup == null)
        {

            Debug.Log("No default stat setup assigned");
            return;
        }

        resources.baseHealth.SetBaseValue(defaultSetup.maxHealth);
        resources.healthRegen.SetBaseValue(defaultSetup.healthRegen);

        major.strength.SetBaseValue (defaultSetup.strength);
        major.agility.SetBaseValue(defaultSetup.agility);
        major.intelligence.SetBaseValue(defaultSetup.intelligence);
        major.vitality.SetBaseValue(defaultSetup.vitality);

        offense.attackSpeed.SetBaseValue(defaultSetup.attackSpeed);
        offense.damage.SetBaseValue(defaultSetup.damage);
        offense.critChance.SetBaseValue(defaultSetup.critChance);
        offense.critPower.SetBaseValue(defaultSetup.critPower);
        offense.armorReduction.SetBaseValue(defaultSetup.armorReduction);

        offense.iceDamage.SetBaseValue(defaultSetup.iceDamage);
        offense.fireDamage.SetBaseValue(defaultSetup.fireDamage);
        offense.lightningDamage.SetBaseValue(defaultSetup.lightningDamage);

        defense.armor.SetBaseValue(defaultSetup.armor);
        defense.evasion.SetBaseValue(defaultSetup.evasion);

        defense.iceRes.SetBaseValue(defaultSetup.iceResistance);
        defense.fireRes.SetBaseValue(defaultSetup.fireResistance);
        defense.lightningRes.SetBaseValue(defaultSetup.lightningResistance);
    }
}
