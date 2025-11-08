using Unity.VisualScripting;
using UnityEngine;

public class Object_ItemPickUp : MonoBehaviour
{
    [SerializeField] private SpriteRenderer sr;
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Collider2D col;

    private Player player;
    private bool playerInRange = false;

    [SerializeField] public Item_DataSO itemData;
    [SerializeField] private Vector2 dropForce = new Vector2(3, 10);

    public ItemModifier[] modifiers { get; private set; }
    public SkillDataItem skillItem { get; private set; }
    
    private UI_PlayerOnItemToolTip playerOnItemToolTip;

    private Inventory_Item itemToAdd;
    private Inventory_Base inventory;

    private void Awake()
    {
        //itemToAdd = new Inventory_Item(itemData);
        playerOnItemToolTip = FindAnyObjectByType<UI_PlayerOnItemToolTip>();
        modifiers = EquipmentData()?.modifiers;
        skillItem = EquipmentData()?.skillDataItem;
    }

    private void OnValidate()
    {
        if (itemData == null)
            return;

        sr = GetComponent<SpriteRenderer>();
        SetupVisuals();
        
    }

    public void SetupItem(Item_DataSO itemData)
    {
        this.itemData = itemData;
        Debug.Log("Item " + this.itemData.itemName);
        SetupVisuals();

        float xDropForce = Random.Range(-dropForce.x, dropForce.x);
        rb.linearVelocity = new Vector2(xDropForce, dropForce.y);
        col.isTrigger = false;
    }

    private void SetupVisuals()
    {
        sr.sprite = itemData.itemIcon;
        gameObject.name = "Object_ItemPickup - " + itemData.itemName;
    }
    private void Update()
    {
        if (!playerInRange || player == null || inventory == null) return;

        // Aquí se revisa si se presionó el botón
        if (player.input.Player.PickUp.WasPressedThisFrame() && inventory.CanAddItem(itemToAdd))
        {

            inventory.AddItem(itemToAdd);
            playerOnItemToolTip.ShowToolTip(false, null);
            Destroy(gameObject);
        }
    }
   
    private void OnTriggerEnter2D(Collider2D collision)
    {

        inventory = collision.GetComponent<Inventory_Base>();
        if (inventory == null)
            return;

        player = collision.GetComponent<Player>();
        /*Inventory_Item*/ itemToAdd = new Inventory_Item(itemData);
        if (player != null)
        {
            
            playerInRange = true;
            playerOnItemToolTip.ShowToolTip(true, transform, this);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.GetComponent<Player>() != null)
        {
            playerInRange = false;
            player = null;
            inventory = null;

            playerOnItemToolTip.ShowToolTip(false, null);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.layer == LayerMask.NameToLayer("Ground") && col.isTrigger == false)
        {
            col.isTrigger = true;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }
    }
    private Equipment_DataSO EquipmentData()
    {
        if (itemData is Equipment_DataSO equipment)
            return equipment;

        return null;
    }

}
