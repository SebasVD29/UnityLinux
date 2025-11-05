using UnityEngine;

public class Player_SkillManager : MonoBehaviour
{


    public Skill_Dash dash {  get; private set; }
    public Skill_Parry parry {  get; private set; }
    public Skill_SwordThrow swordThrow {  get; private set; }
    public Skill_Base[] allSkills { get; private set; }

    private void Awake()
    {
        dash = GetComponentInChildren<Skill_Dash>();
        parry = GetComponentInChildren<Skill_Parry>();
        swordThrow = GetComponentInChildren<Skill_SwordThrow>();

        allSkills = GetComponentsInChildren<Skill_Base>();
    }

    public void ReduceAllSkillCooldownBy(float amount)
    {
        foreach (var skill in allSkills)
            skill.ReduceCooldownBy(amount);
    }

    public Skill_Base GetSkillByType(SkillType type)
    {
        switch (type)
        {
            case SkillType.Dash: return dash;
            case SkillType.Parry: return parry;
            case SkillType.SwordThrow: return swordThrow;


            default:
                Debug.Log($"skill type {type}, no existe");
                return null;
        }
    }

}
