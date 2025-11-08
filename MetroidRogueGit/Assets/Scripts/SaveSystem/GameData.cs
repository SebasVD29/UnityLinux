using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[Serializable]
public class GameData 
{
    public bool LevelTutorial = false;
    public bool UITutorial = false;

    public List<Inventory_Item> itemList;
    public SerializableDictionary<string, int> inventory; // itemSaveId -> stackSize


    public SerializableDictionary<string, ItemType> equipedItems; //  itemSaveId -> slot for item

    public SerializableDictionary<string, bool> skillStatus; //  skill name -> unlock status
    public SerializableDictionary<SkillType, SkillUnlockType> skillUnlocks; // skill type -> upgrade type

    public SerializableDictionary<string, bool> unlockedCheckpoints; // checkpoint id -> unlocked status
    public SerializableDictionary<string, Vector3> inScenePortals; // scene name > portal position

    public string portalDestinationSceneName;
    public bool returningFromTown;

    public string lastScenePlayed;
    public Vector3 lastPlayerPosition;

    public GameData()
    {
        inventory = new SerializableDictionary<string, int>();
    

        equipedItems = new SerializableDictionary<string, ItemType>();

        skillStatus = new SerializableDictionary<string, bool>();
        skillUnlocks = new SerializableDictionary<SkillType, SkillUnlockType>();

        unlockedCheckpoints = new SerializableDictionary<string, bool>();
        inScenePortals = new SerializableDictionary<string, Vector3>();
    }

}
