using UnityEngine;

public class Chest : MonoBehaviour, IDamageable
{
    private Animator animator => GetComponentInChildren<Animator>();
    private Rigidbody2D rb => GetComponentInChildren<Rigidbody2D>();
    private Entity_VFX vfx => GetComponent<Entity_VFX>();
    private Entity_DropManager dropManager => GetComponent<Entity_DropManager>();   


    [Header("Open Details")]
    [SerializeField] private Vector2 knockback;
    [SerializeField] private  bool canDropItems = true;

    [Header("Tutorial References")]
    [SerializeField] private GameObject tutorialOpenChest;      
    [SerializeField] private GameObject nextTutorialPickUpItem;
    public bool TakeDamage(float damage, float elemental, Transform damageDealer, ElementType element)
    {
        if (canDropItems == false)
            return false;

        canDropItems = false;
        dropManager?.DropItems();
        vfx.PlayOnDamageVfx();
        animator.SetBool("chestOpen", true);
        rb.linearVelocity = knockback;
        rb.angularVelocity = Random.Range(-200f, 200f);
        OnChestOpened();

        return true;
    }
    public void OnChestOpened()
    {
        if (nextTutorialPickUpItem == null || tutorialOpenChest == null)
            return;

        if (nextTutorialPickUpItem != null)
            nextTutorialPickUpItem.SetActive(true);

        if (tutorialOpenChest != null)
            tutorialOpenChest.SetActive(false);
    }
}
