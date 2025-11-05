using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(menuName = "MetroRogue/Skill Data", fileName = "Skill Data")]
public class Skill_DataSO : ScriptableObject
{
    [Header("Skill description")]
    public string displayName;
    [TextArea]
    public string description;
    public Sprite icon;

    [Header("Unlock & Upgrade")]
    public int cost;
    public bool unlockedByDefault;
    public SkillType skillType;
    public UnlockData unlockData;

    [Header("Input Action")]
    public List<InputActionReference> inputActions;
  

}

[Serializable]
public class UnlockData
{
    public SkillUnlockType unlockType;
    public float cooldown;
    public DamageScaleData damageScaleData;
}
