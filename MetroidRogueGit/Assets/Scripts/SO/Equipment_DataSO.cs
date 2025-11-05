using System;
using UnityEngine;

[CreateAssetMenu(menuName = "MetroRogue/Item Data/Equipment Item", fileName = "Equipment Data - ")]
public class Equipment_DataSO : Item_DataSO
{
    [Header("Item Modifiers")]
    public ItemModifier[] modifiers;

    [Header("Skill Unlock")]
    public SkillDataItem skillDataItem;

}

[Serializable]
public class ItemModifier
{
    public StatType statType;
    public float value;
}

[Serializable]
public class SkillDataItem
{
    public Skill_DataSO skillData;
    
}