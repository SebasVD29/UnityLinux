using System.Collections.Generic;
using UnityEngine;
using System;

public class Inventory_Base : MonoBehaviour, ISaveable
{
    protected Player player;
    public event Action OnInventoryChange;
    public int maxInventorySize = 10;
    
    public List<Inventory_Item> itemList = new List<Inventory_Item>();//Lista de objetos del inventario

    [Header("ITEM DATA BASE")]
    [SerializeField] protected ItemList_DataSO itemDataBase; //Lista de todos los objetos del juego


    protected virtual void Awake()
    {

    }
    public bool CanAddItem(Inventory_Item itemToAdd)
    {
        return  itemList.Count < maxInventorySize;
    }
    public void AddItem(Inventory_Item itemToAdd)
    {
        Inventory_Item itemInInventory = FindItem(itemToAdd);

        if(itemInInventory == null)
            itemList.Add(itemToAdd);


        OnInventoryChange?.Invoke();
    }


    public void RemoveOneItem(Inventory_Item itemToRemove)
    {
        Inventory_Item itemInInventory = itemList.Find(item => item == itemToRemove);

        if (itemInInventory.stackSize <= 1)
            itemList.Remove(itemToRemove);

        OnInventoryChange?.Invoke();
    }
    public Inventory_Item FindItem(Inventory_Item itemToFind)
    {
        return itemList.Find(item => item == itemToFind);
    }

    public void TriggerUpdateUI() => OnInventoryChange?.Invoke();

    public virtual void LoadData(GameData data)
    {
        
    }

    public virtual void SaveData(ref GameData data)
    {
        
    }
}
