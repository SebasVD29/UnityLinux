using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_PlayerOnItemToolTip : UI_ToolTip
{
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemInfo;
    [SerializeField] private TextMeshProUGUI itemSkillInput;

    public void ShowToolTip(bool show, Transform targetRect, Object_ItemPickUp itemToPickUp)
    {
        base.ShowToolTip(show, itemToPickUp.transform);

        itemName.text = itemToPickUp.itemData.itemName;
        itemType.text = itemToPickUp.itemData.itemType.ToString();
        itemInfo.text = GetItemInfo(itemToPickUp);
        itemSkillInput.text = GetItemInfoInputs(itemToPickUp);
    }

    public string GetItemInfo(Object_ItemPickUp item)
    {

        StringBuilder sb = new StringBuilder();

        sb.AppendLine("");

        if (item.itemData.itemType != ItemType.skills)
        {
            foreach (var mod in item.modifiers)
            {
                string modType = GetStatNameByType(mod.statType);
                string modValue = IsPercentageStat(mod.statType) ? mod.value.ToString() + "%" : mod.value.ToString();
                sb.AppendLine("+ " + modValue + " " + modType);

            }
        }
        else
        {
            string itemDescription = item.skillItem.skillData.description;
            sb.AppendLine("+ " + itemDescription);
        }


        //if (item.itemEffect != null)
        //{
        //    sb.AppendLine("");
        //    sb.AppendLine("Unique effect:");
        //    sb.AppendLine(item.itemEffect.effectDescription);
        //}

        sb.AppendLine("");
        sb.AppendLine("");

        return sb.ToString();
    }
    public string GetItemInfoInputs(Object_ItemPickUp item)
    {
        // Si no es skill o faltan datos, no devuelve nada
        if (item == null || item.itemData == null || item.itemData.itemType != ItemType.skills)
            return string.Empty;

        var skillData = item.skillItem?.skillData;
        if (skillData == null)
            return string.Empty;

        StringBuilder sb = new StringBuilder();
        

        // Lista de textos legibles para cada input
        List<string> inputStrings = new List<string>();

        if (skillData.inputActions != null)
        {
            foreach (var inputAction in skillData.inputActions)
            {
                if (inputAction?.action == null)
                    continue;

                string bindingText = GetBindingText(inputAction);
                if (!string.IsNullOrEmpty(bindingText))
                    inputStrings.Add(bindingText);
            }
        }

        // Solo muestra la línea si hay inputs válidos
        if (inputStrings.Count > 0)
            sb.AppendLine($"Input: {string.Join(", ", inputStrings)}");

        sb.AppendLine();
        return sb.ToString();
    }

    private string GetBindingText(InputActionReference inputActionRef)
    {
        if (inputActionRef == null || inputActionRef.action == null)
            return string.Empty;

        var action = inputActionRef.action;

        // Prioridad al mando
        if (Gamepad.current != null)
        {
            string gamepadBinding = action.GetBindingDisplayString(group: "Gamepad");
            if (!string.IsNullOrEmpty(gamepadBinding))
                return gamepadBinding;
        }

        // Si no hay mando usa teclado/mouse
        string kbBinding = action.GetBindingDisplayString(group: "Keyboard&Mouse");
        if (!string.IsNullOrEmpty(kbBinding))
            return kbBinding;

        // Si todo falla, muestra un fallback genérico
        return action.name;
    }

    private string GetStatNameByType(StatType type)
    {
        switch (type)
        {
            case StatType.MaxHealth: return "Max Health";
            case StatType.HealthRegen: return "Health Regeneration";
            case StatType.Armor: return "Armor";
            case StatType.Evasion: return "Evasion";

            case StatType.Strength: return "Strength";
            case StatType.Agility: return "Agility";
            case StatType.Intelligence: return "Intelligence";
            case StatType.Vitality: return "Vitality";

            case StatType.AttackSpeed: return "Attack Speed";
            case StatType.Damage: return "Damage";
            case StatType.CritChance: return "Critical Chance";
            case StatType.CritPower: return "Critical Power";
            case StatType.ArmorReduction: return "Armor Reduction";

            case StatType.FireDamage: return "Fire Damage";
            case StatType.IceDamage: return "Ice Damage";
            case StatType.LightningDamage: return "Lightning Damage";

            case StatType.IceResistance: return "Ice Resistance";
            case StatType.FireResistance: return "Fire Resistance";
            case StatType.LightningResistance: return "Lightning Resistance";
            default: return "Unknown Stat";
        }
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
