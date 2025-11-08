using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UI_Inventory : MonoBehaviour, ISaveable
{
    public bool isTutorialComplete = false;
    [SerializeField] protected Image UI_TutorialInventoryParent;

    private UI_ItemSlot[] uiItemSlots;
    private UI_EquipSlot[] uiEquipSlots;
    private List<UI_ItemSlot> allSlots = new List<UI_ItemSlot>();
    private Inventory_Player inventory;

    [SerializeField] private Transform uiItemSlotParent;
    [SerializeField] private Transform uiEquipSlotParent;

    [Header("UI BG slot Setup")]
    [SerializeField] private Sprite imageNormalBg;
    [SerializeField] private Sprite imageBgHighlight;

    [Header("EventSystem setup")]
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject firstSelectedSlot;

    private void Awake()
    {
        uiItemSlots = uiItemSlotParent.GetComponentsInChildren<UI_ItemSlot>();
        uiEquipSlots = uiEquipSlotParent.GetComponentsInChildren<UI_EquipSlot>();

        inventory = FindFirstObjectByType<Inventory_Player>();

        inventory.OnInventoryChange += UpdateUI;

        UpdateUI();
    }
    private void Update()
    {
 
    }
    private void OnEnable()
    {
        if (inventory == null)
            return;

        UpdateUI();
  
        StartTutorial();

        // 🔹 Al abrir el inventario, fija el primer slot seleccionado
        if (eventSystem == null)
            eventSystem = EventSystem.current;

        if (firstSelectedSlot != null)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(firstSelectedSlot);
        }
    }
    private void OnDisable()
    {
        if (eventSystem != null)
            eventSystem.SetSelectedGameObject(null);
    }
    private void HighlightSlot(int index)
    {
        for (int i = 0; i < allSlots.Count; i++)
        {
            allSlots[i].SetHighlight(i == index, imageNormalBg, imageBgHighlight);
        }
    }

    private void UpdateUI()
    {
        UpdateInventorySlots();
        UpdateEquipmentSlots();
    }

    void UpdateInventorySlots()
    {
        List<Inventory_Item> itemList = inventory.itemList;

        for (int i = 0; i< uiItemSlots.Length; i++)
        {
            if (i < itemList.Count)
            {
                uiItemSlots[i].UpdateSlot(itemList[i]);
            }
            else
            {
                uiItemSlots[i].UpdateSlot(null);
            }
        }
    }

    private void UpdateEquipmentSlots()
    {
        List<Inventory_EquipmentSlot> playerEquipList = inventory.equipList;

        for (int i = 0; i < uiEquipSlots.Length; i++)
        {
            var playerEquipSlot = playerEquipList[i];

            if (playerEquipSlot.HasItem() == false)
                uiEquipSlots[i].UpdateSlot(null);
            else
                uiEquipSlots[i].UpdateSlot(playerEquipSlot.equipedItem);
        }
    }


    public void StartTutorial()
    {
        if (isTutorialComplete) return;


        if (UI_TutorialInventoryParent != null)
        {
            UI_TutorialInventoryParent.gameObject.SetActive(true);

            // Configurar controles
            var tutorial = UI_TutorialInventoryParent.GetComponent<UI_GeneralInventorylTutorial>();

            if (tutorial != null)
                tutorial.SetupControlsUI();
        }
    }
    public void MarkTutorialAsComplete()
    {
        isTutorialComplete = true;
        SaveManager.instance?.SaveGame();
    }

    public void LoadData(GameData data)
    {
        isTutorialComplete = data.UITutorial;

        if (isTutorialComplete && UI_TutorialInventoryParent != null)
            UI_TutorialInventoryParent.gameObject.SetActive(false);
    }

    public void SaveData(ref GameData data)
    {
        data.UITutorial = isTutorialComplete; ;
    }
}
