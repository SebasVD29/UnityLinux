using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class UI_ItemSlot : MonoBehaviour, 
    IPointerDownHandler, 
    IPointerEnterHandler, 
    IPointerExitHandler, 
    ISelectHandler,
    IDeselectHandler,
    ISubmitHandler
{
    public Inventory_Item itemInSlot {  get; private set; }
    protected Inventory_Player inventory;
    protected UI ui;
    protected RectTransform rect;


    [Header("UI slot Setup")]
    [SerializeField] private GameObject defaultIcon;
    [SerializeField] private Image itemIcon;
    [SerializeField] private Image itemIconBG;
    [SerializeField] private Sprite normalSprite;
    [SerializeField] private Sprite highlightSprite;

  

    protected void Awake()
    {
        ui = GetComponentInParent<UI>();
        rect = GetComponent<RectTransform>();
        inventory = FindAnyObjectByType<Inventory_Player>();

    }

    public void OnSelect(BaseEventData eventData)
    {
        if (itemIconBG != null)
            itemIconBG.sprite = highlightSprite;

        
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (itemIconBG != null)
            itemIconBG.sprite = normalSprite;
    }
    public virtual void OnSubmit(BaseEventData eventData)
    {
        Debug.Log($"Submit on {itemInSlot}");
        if (itemInSlot == null)
            return;

        inventory.TryEquipItem(itemInSlot);

        if (itemInSlot == null)
            ui.itemToolTip.ShowToolTip(false, null);

    }
    public virtual void OnPointerDown(PointerEventData eventData)
    {
        if (itemInSlot == null )
            return;

        inventory.TryEquipItem(itemInSlot);

        if(itemInSlot == null)
            ui.itemToolTip.ShowToolTip(false, null);
    }

    public void UpdateSlot(Inventory_Item item)
    {
        itemInSlot = item;

        if (defaultIcon != null)
            defaultIcon.gameObject.SetActive(itemInSlot == null);

        if (itemInSlot == null)
        {
            itemIcon.color = Color.clear;
            //defaultIcon.gameObject.SetActive(true);
            return;
        }

    
        Color color = Color.white;
        color.a = .9f;
        itemIcon.color = color;
        itemIcon.sprite = itemInSlot.itemData.itemIcon;
    }
    public virtual void OnPointerEnter(PointerEventData eventData)
    {
        if (itemInSlot == null) return;

        ui.itemToolTip.ShowToolTip(true, rect, itemInSlot);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ui.itemToolTip.ShowToolTip(false, null);
    }

    public void SetHighlight(bool active, Sprite normal, Sprite highlight)
    {
        if (itemIconBG != null)
            itemIconBG.sprite = active ? highlight : normal;
    }

}
