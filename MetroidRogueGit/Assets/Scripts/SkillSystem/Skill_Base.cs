using System.Collections.Generic;
using UnityEngine;

public class Skill_Base : MonoBehaviour
{
    protected Entity_VFX vfx;

    public Player_SkillManager skillManager { get; private set; }
    public Player player { get; private set; }
    public DamageScaleData damageScaleData { get; private set; }

    [Header("Skil details")]
    public List<Skill_DataSO> skillData;

    [Header("General Details")]
    [SerializeField] protected SkillType skillType;
    [SerializeField] protected SkillUnlockType skillUnlockType;
    public float cooldown;
    private float lastTimeUsed;


    protected virtual void Awake()
    {
        skillManager = GetComponentInParent<Player_SkillManager>();
        vfx = GetComponentInParent<Entity_VFX>();
        player = GetComponentInParent<Player>();
        lastTimeUsed = lastTimeUsed - cooldown;
        damageScaleData = new DamageScaleData();


    }

    public virtual void TryUseSkill()
    {

    }


    public virtual bool CanUseSkill()
    {
   
        if (skillUnlockType == SkillUnlockType.None)
            return false;

        if (OnCooldown())
        {
            Debug.Log("On Cooldown Skill - Skill_Base");
            return false;
        }

        return true;
    }
    
    public void SetSkillUnlock(Skill_DataSO skillData)
    {
        UnlockData unlock = skillData.unlockData;

        skillUnlockType = unlock.unlockType;
        cooldown = unlock.cooldown;
        damageScaleData = unlock.damageScaleData;

        player.ui.inGameUI.GetSkillSlot(skillType).SetupSkillSlot(skillData);
        ResetCooldown();
    }

    public void SetSkillLock(UnlockData unlock)
    {
        //UnlockData unlock = skillData.unlockData;

        skillUnlockType = unlock.unlockType;
        cooldown = unlock.cooldown;
        damageScaleData = unlock.damageScaleData;

        var uiSlot = player.ui.inGameUI.GetSkillSlot(skillType);
        if (uiSlot != null)
            uiSlot.ClearSlot();

    }
    public void SetSkillOnCooldown()
    {
        player.ui.inGameUI.GetSkillSlot(skillType).StartCooldown(cooldown);
        lastTimeUsed = Time.time;
    }
    public void ResetCooldown()
    {
        player.ui.inGameUI.GetSkillSlot(skillType).ResetCooldown();
        lastTimeUsed = Time.time - cooldown;
    }

    protected bool Unlocked(SkillUnlockType uplockToCheck) => skillUnlockType == uplockToCheck;

    protected bool OnCooldown() => Time.time < lastTimeUsed + cooldown;
    public void ReduceCooldownBy(float cooldownReduction) => lastTimeUsed = lastTimeUsed + cooldownReduction;

    public SkillUnlockType GetUpgrade() => skillUnlockType;
    public SkillType GetSkillType() => skillType;

}
