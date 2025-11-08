using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class Inventory_Player : Inventory_Base
{
    public event Action<Skill_DataSO> OnSkillEquipped;
    public event Action<Skill_DataSO> OnSkillUnequipped;
    public List<Inventory_EquipmentSlot> equipList;//lista de objetos equipados

    protected override void Awake()
    {
        base.Awake();
        player = GetComponent<Player>();
    }


    public void TryEquipItem(Inventory_Item item)
    {
        var inventoryItem = FindItem(item);
        var matchingSlots = equipList.FindAll(slot => slot.slotType == item.itemData.itemType);

        // STEP 1 : Try to find empty slot and equip item
        foreach (var slot in matchingSlots)
        {
            if (slot.HasItem() == false)
            {
                if (inventoryItem.skillEquipmentItem.skillData == null)
                {
                    Debug.Log("empty slot equipar item!");
                    EquipItem(inventoryItem, slot);
                }
                else
                {
                    Debug.Log("empty slot equipar skill!");
                    EquipSkill(inventoryItem, slot);
                }


                return;
            }
        }

        // STEP 2: No empty slots ? Replace first one
        var slotToReplace = matchingSlots[0];
        var itemToUnequip = slotToReplace.equipedItem;

        UnequipItem(itemToUnequip, slotToReplace != null);

        if (inventoryItem.skillEquipmentItem.skillData == null)
        {
            Debug.Log("Replace slot equipar item!");
            EquipItem(inventoryItem, slotToReplace);
        }
        else
        {
            Debug.Log("Replace slot equipar skill!");
            EquipSkill(inventoryItem, slotToReplace);
        }
    }

    void EquipItem(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
        float savaedHealthPercent = player.health.GetHealthPercent();

        slot.equipedItem = itemToEquip;
        slot.equipedItem.AddModifiers(player.stats);
  
        player.health.SetHealthToPercent(savaedHealthPercent);
        
        RemoveOneItem(itemToEquip);
    }
    void EquipSkill(Inventory_Item itemToEquip, Inventory_EquipmentSlot slot)
    {
       
        var skillType = itemToEquip.skillEquipmentItem.skillData.skillType;
        var skillUnlockData = itemToEquip.skillEquipmentItem.skillData;

        var skill = player.skillManager.GetSkillByType(skillType);
        if (skill != null)
        {
            skill.SetSkillUnlock(skillUnlockData);
            Debug.Log($"Skill {skillType} - {skillUnlockData} equipada y habilitada.");
            OnSkillEquipped?.Invoke(skillUnlockData);
        }

        slot.equipedItem = itemToEquip;
        RemoveOneItem(itemToEquip);
    }
    public void UnequipItem(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        if (itemToUnequip == null)
        {
            Debug.LogWarning("UnequipItem: Se intentó desequipar un item nulo.");
            return;
        }

        // Si no hay espacio y no estamos reemplazando, no se puede desequipar
        if (!CanAddItem(itemToUnequip) && !replacingItem)
        {
            Debug.Log("No hay espacio en el inventario para desequipar el ítem.");
            return;
        }

        float savedHealthPercent = player.health.GetHealthPercent();
        var slotToUnequip = equipList.Find(slot => slot.equipedItem == itemToUnequip);

        if (itemToUnequip.skillEquipmentItem.skillData != null)
        {
            UnequipSkill(itemToUnequip, replacingItem);
        }


        if (slotToUnequip != null)
            slotToUnequip.equipedItem = null;

        itemToUnequip.RemoveModifiers(player.stats);
        //itemToUnequip.RemoveItemEffect();

        player.health.SetHealthToPercent(savedHealthPercent);
        AddItem(itemToUnequip);
    }
    void UnequipSkill(Inventory_Item itemToUnequip, bool replacingItem = false)
    {
        Debug.Log("Desequipar skill!");

        var skillType = itemToUnequip.skillEquipmentItem.skillData.skillType;
        var skillUnlockData = itemToUnequip.skillEquipmentItem.skillData;

        var skillToUnequip = new UnlockData
        {
            unlockType = SkillUnlockType.None,
            cooldown = 0,
            damageScaleData = new DamageScaleData()
        };

        var skill = player.skillManager.GetSkillByType(skillType);
        if (skill != null)
        {
            skill.SetSkillLock(skillToUnequip);
            Debug.Log($"Skill {skillType} - {skillToUnequip} desequipada y deshabilitada.");
            OnSkillUnequipped?.Invoke(skillUnlockData);
        }

    }
    public void ResetAllInventoryAndEquipment()
    {
      
        foreach (var slot in equipList)
        {
            if (slot.HasItem() == true )
                UnequipItem(slot.equipedItem);
        }
        // Quitar todos los ítems del inventario
        itemList.Clear();
       
        // 🔸 Actualizar la UI
        TriggerUpdateUI();
 
    }
    public override void LoadData(GameData data)
    {

        foreach (var entry in data.inventory)
        {
            string saveId = entry.Key;
            int stackSize = 1;

            Item_DataSO itemData = itemDataBase.GetItemData(saveId);

            if (itemData == null)
            {
                Debug.LogWarning("Item not found: " + saveId);
                continue;
            }


            for (int i = 0; i < stackSize; i++)
            {
                Inventory_Item itemToLoad = new Inventory_Item(itemData);
                AddItem(itemToLoad);
            }
        }

        foreach (var entry in data.equipedItems)
        {
            string saveId = entry.Key;
            ItemType equipmentSlotType = entry.Value;

            Item_DataSO itemData = itemDataBase.GetItemData(saveId);
            if (itemData == null)
            {
                Debug.LogWarning("Item not found (equipped): " + saveId);
                continue;
            }

            Inventory_Item itemToLoad = new Inventory_Item(itemData);
            var slot = equipList.Find(s => s.slotType == equipmentSlotType && s.HasItem() == false);
            if (slot == null)
            {
                Debug.LogWarning($"No slot found for type {equipmentSlotType}");
                continue;
            }

            slot.equipedItem = itemToLoad;
            slot.equipedItem.AddModifiers(player.stats);

        }
        foreach (var skill in player.skillManager.allSkills)
        {
            SkillType type = skill.GetSkillType();

            if (data.skillUnlocks.TryGetValue(type, out SkillUnlockType unlockType))
            {
                Skill_DataSO skillData = skill.skillData
                    .FirstOrDefault(sd => sd.unlockData.unlockType == unlockType);

                if (skillData != null)
                {
                    skill.SetSkillUnlock(skillData);
                   // Debug.Log($"Skill {type} restaurada con unlock {unlockType}");
                }
                else
                {
                    Debug.LogWarning($"No se encontró Skill_DataSO para {type} con unlock {unlockType}");
                }
            }
            else
            {
                if (skill.skillData != null && skill.skillData.Count > 0)
                    skill.SetSkillLock(skill.skillData[0].unlockData);
            }
        }
        player.health.UpdateOnLoadCurrentHealth();

        TriggerUpdateUI();
    }

    public override void SaveData(ref GameData data)
    {
        data.inventory.Clear();
        data.equipedItems.Clear();
        data.skillStatus.Clear();
        data.skillUnlocks.Clear();

        foreach (var item in itemList)
        {
            if (item != null && item.itemData != null)
            {
                string saveId = item.itemData.saveId;

                if (data.inventory.ContainsKey(saveId) == false)
                    data.inventory[saveId] = 0;

                data.inventory[saveId] += 1;
            }
        }

        foreach (var slot in equipList)
        {
            if (slot.HasItem())
                data.equipedItems[slot.equipedItem.itemData.saveId] = slot.slotType;
        }

        foreach (var skill in player.skillManager.allSkills)
        {
            SkillType skillType = skill.GetSkillType();
            SkillUnlockType unlockType = skill.GetUpgrade();

            data.skillUnlocks[skillType] = unlockType;

            // (Opcional) si querés guardar también si está activa/desbloqueada
            data.skillStatus[skillType.ToString()] = (unlockType != SkillUnlockType.None);
        }
    }
}
