using UnityEditor;
using UnityEngine;

[CreateAssetMenu(menuName = "MetroRogue/Item Data/Material Item", fileName = "Material Data")]
public class Item_DataSO : ScriptableObject
{
    public string saveId;

    [Header("Item details")]
    public string itemName;
    public Sprite itemIcon;
    public ItemType itemType;

    [Header("Drop Details")]
    [Range(0, 1000)]
    public int itemRarity = 100;
    [Range(0, 100)]
    public float dropChance;
    [Range(0, 100)]
    public float maxDropChance = 65f;

    private void OnValidate()
    {
        dropChance = GetDropChance();

#if UNITY_EDITOR
        string path = AssetDatabase.GetAssetPath(this);
        saveId = AssetDatabase.AssetPathToGUID(path);
#endif
    }

    public float GetDropChance()
    {
        float maxRarity = 1000;
        float chance = (maxRarity - itemRarity + 1) / maxRarity * 100;

        return Mathf.Min(chance, maxDropChance);
    }
}
