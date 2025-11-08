using UnityEngine;

public class Object_BulletHell : MonoBehaviour
{
    private Collider2D col;
    private Rigidbody2D rb;
    private Entity_Combat combat;
    private Animator anim;

    private DamageScaleData damageScaleData;

    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private float lifeTime = 3f;
    float timer;

   

    void Update()
    {
        timer += Time.deltaTime;

        if (timer >= lifeTime)
            Destroy(gameObject);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
     
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)
        {
            combat.PerformAttackOnTarget(collision.transform);
            
        }
    }
    public void SetupBulletHell( Entity_Combat combat)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();

        this.combat = combat;

      

    }
}
