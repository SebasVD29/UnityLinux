using UnityEngine;

public class UI_SkillBar : MonoBehaviour
{
    [SerializeField] private UI_SkillSlot[] skillSlots;

    public void EquipSkill(Skill_DataSO skillData)
    {
        foreach (var slot in skillSlots)
        {
            if (slot.IsEmpty())
            {
                slot.SetupSkillSlot(skillData);
                return;
            }
        }
        Debug.LogWarning("No free slot in SkillBar!");
    }

    public void RemoveSkill(Skill_DataSO skillUnlockType)
    {
        foreach (var slot in skillSlots)
        {
            if (slot.GetSkillUnlockType() == skillUnlockType.unlockData.unlockType)
            {
                slot.ClearSlot();
                return;
            }
        }
    }
}
