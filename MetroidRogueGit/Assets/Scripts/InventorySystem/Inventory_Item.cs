using System;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class Inventory_Item 
{
    private string itemId;

    public Item_DataSO itemData;
    public ItemModifier[] modifiers {  get; private set; }
    public SkillDataItem skillEquipmentItem {  get; private set; }

    public int stackSize = 1;

    public Inventory_Item(Item_DataSO itemData)
    {
        this.itemData = itemData;

        modifiers = EquipmentData()?.modifiers;
        skillEquipmentItem = EquipmentData()?.skillDataItem;
        itemId = itemData.itemName + " - " + Guid.NewGuid();
    }


    public void AddModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.AddModifier(mod.value, itemId);
  
        }
    }

    public void RemoveModifiers(Entity_Stats playerStats)
    {
        foreach (var mod in modifiers)
        {
            Stat statToModify = playerStats.GetStatByType(mod.statType);
            statToModify.RemoveModifier(itemId);

        }
    }
    private Equipment_DataSO EquipmentData()
    {
        if(itemData is Equipment_DataSO equipment)
            return equipment;

        return null;
    }
    private bool IsPercentageStat(StatType type)
    {
        switch (type)
        {
            case StatType.CritChance:
            case StatType.CritPower:
            case StatType.ArmorReduction:
            case StatType.IceResistance:
            case StatType.FireResistance:
            case StatType.LightningResistance:
            case StatType.AttackSpeed:
            case StatType.Evasion:
                return true;
            default:
                return false;
        }
    }

}
