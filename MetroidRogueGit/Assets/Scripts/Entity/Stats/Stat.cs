using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Stat 
{
    [SerializeField] private float baseValue;
    [SerializeField] private List<Stat_Modifier> modifiers = new List<Stat_Modifier>();

    private float finalValue;
    private bool needToCalculate = true;

    public float GetValue()
    {
        if (needToCalculate)
        {
            finalValue = GetFinalValue();
            needToCalculate = false;
        }


        return finalValue;
    }

    public void AddModifier(float value, string source)
    {
        Stat_Modifier modToAdd = new Stat_Modifier(value, source);
        modifiers.Add(modToAdd);
        needToCalculate = true;
    }

    public void RemoveModifier(string source)
    {
        modifiers.RemoveAll(modifier => modifier.source == source);
        needToCalculate = true;
    }

    private float GetFinalValue()
    {
        float finalValue = baseValue;

        foreach (var modifier in modifiers)
        {
            finalValue = finalValue + modifier.value;
        }

        return finalValue;
    }

    public void SetBaseValue(float value) => baseValue = value;

}

[Serializable]
public class Stat_Modifier
{
    public float value;
    public string source;

    public Stat_Modifier(float value, string source)
    {
        this.value = value;
        this.source = source;
    }
}
