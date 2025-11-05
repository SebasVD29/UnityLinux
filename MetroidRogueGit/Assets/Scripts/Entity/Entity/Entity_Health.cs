
using System;

using UnityEngine;
using UnityEngine.UI;

public class Entity_Health : MonoBehaviour, IDamageable
{
    //public event Action OnTakingDamage;
    public event Action OnHealthUpdate;

    private Entity_VFX Vfx;
    protected Entity_Stats stats;
    private Entity entity;
    private Slider healthBar;
    private Entity_DropManager dropManager;

    [Header("Health")]
    [SerializeField] protected float currentHealth;
    [SerializeField] protected bool isDead;
    [SerializeField] private float regenInterval = 1;
    [SerializeField] private bool canRegenHealth = true;

    [Header("On Damage Knockback")]
    [SerializeField] private Vector2 knockbackPower = new Vector2(1.5f, 2.5f);
    [SerializeField] private Vector2 heacyKnockbackPower = new Vector2(7, 7);
    [SerializeField] private float knockbackDuration =.2f;
    [SerializeField] private float heavyKnockbackDuration = .5f;

    [Header("On Heavy Damage")]
    [SerializeField] private float heavyDamageThreshold = .3f;

    protected virtual void Awake()
    {
        Vfx = GetComponent<Entity_VFX>();
        entity = GetComponent<Entity>();
        healthBar = GetComponentInChildren<Slider>();
        stats = GetComponent<Entity_Stats>();
        dropManager = GetComponent<Entity_DropManager>();

        SetupHealth();

    }
   
    void SetupHealth()
    {
        if (stats == null)
            return;

        currentHealth = stats.GetBaseHealth();
        OnHealthUpdate += UpdateHealthBar;

        UpdateHealthBar();
        InvokeRepeating(nameof(RegenerateHealth), 0, regenInterval);
    }
    public void UpdateOnLoadCurrentHealth()
    {
        if (stats == null)
            return;

        if (currentHealth != stats.GetTotalHealth())
            currentHealth = stats.GetTotalHealth();

        UpdateHealthBar();
    }
    void UpdateHealthBar()
    {
        if (healthBar == null)
            return;

        healthBar.value = currentHealth / stats.GetTotalHealth();
    }
    public float GetHealthPercent() 
    {

        return currentHealth / stats.GetTotalHealth();
    }
    public float GetCurrentHealth() => currentHealth;
    public void SetHealthToPercent(float percent)
    {
        currentHealth = stats.GetTotalHealth() * Mathf.Clamp01(percent);
        OnHealthUpdate?.Invoke();
    }
    
    public virtual bool TakeDamage(float damage, float elemental, Transform damageDealer, ElementType element)
    {
        if (isDead)
            return false;

        if (AttackEvaded())
            return false;

        Entity_Stats attackerStats = damageDealer.GetComponent<Entity_Stats>();
        float armorReduction = attackerStats != null ? attackerStats.GetArmorReduction() : 0;

        float mitigation = stats.GetArmorMitigation(armorReduction);
        float physicalDamageTaken = damage * (1 - mitigation);

        float resistance = stats.GetElementalResistance(element);
        float elementDamageTaken = elemental * (1 - resistance);

        TakeKnockback(damageDealer, physicalDamageTaken);
        ReduceHealth(physicalDamageTaken + elementDamageTaken);

        return true;

    }
    public void ResetHealth()
    {
        if (stats == null)
            return;

        currentHealth = stats.GetTotalHealth(); // Restaurar la vida completa
        isDead = false;                          // Resetear estado de muerto
        OnHealthUpdate?.Invoke();                // Actualizar barra de vida
    }

    private void RegenerateHealth()
    {
        if (canRegenHealth == false)
            return;

        float regenAmount = stats.resources.healthRegen.GetValue();
        IncreaseHealth(regenAmount);

    }

    public void IncreaseHealth(float healAmount)
    {
        if (isDead) 
            return;

        float newHealth = currentHealth + healAmount;
        float maxHealth = stats.GetTotalHealth();

        currentHealth = Mathf.Min(newHealth, maxHealth);
        OnHealthUpdate?.Invoke();
        
    }

    public void ReduceHealth(float damage)
    {

        currentHealth -= damage;

        Vfx?.PlayOnDamageVfx();
        OnHealthUpdate?.Invoke();

        if (currentHealth < 0)
            Die();
    }
    
    protected virtual void Die()
    {
        isDead = true;
        entity?.EntityDeath();
        //dropManager?.DropItems();
    }
    private void TakeKnockback(Transform damageDealer, float finalDamage)
    {
        Vector2 knockback = CalculateKnockback(finalDamage, damageDealer);
        float duration = CalculateDuration(finalDamage);

        entity?.ReciveKnockback(knockback, duration);
    }

    private Vector2 CalculateKnockback(float damage, Transform damageDealer)
    {
        int direction = transform.position.x > damageDealer.position.x ? 1 : -1;

        Vector2 knockback = IsHeavyDamage(damage) ? heacyKnockbackPower : knockbackPower;

        knockback.x = knockback.x * direction;


        return knockback;
    }
    private bool IsHeavyDamage(float damage)
    {
        if (stats == null)
            return false;
        else
            return damage / stats.GetTotalHealth() > heavyDamageThreshold;
    }
    private bool AttackEvaded()
    {
        if (stats == null)
            return false;
        else
            return UnityEngine.Random.Range(0, 100) < stats.GetEvasion();
    }
    float CalculateDuration(float damage) => IsHeavyDamage(damage) ? heavyKnockbackDuration : knockbackDuration;
  
    public virtual void InstaKill(int parryCountToInstaKill, int originalParryCount)
    {

    }

    public virtual void StealHealth(Player stealerEntity, Enemy loseEntity)
    {


    }
}
