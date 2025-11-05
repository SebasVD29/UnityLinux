using UnityEngine;

public class EnemyMage_Fireball : MonoBehaviour
{
    private Collider2D col;
    private Rigidbody2D rb;
    private Entity_Combat combat;
    private Animator anim;

    private Vector2 direction;


    [SerializeField] private float speed = 6f;
    [SerializeField] private LayerMask whatIsTarget;
    [SerializeField] private float lifeTime = 3f;

    private DamageScaleData damageScaleData;

    public void SetupFireBall(Vector2 targetPosition, Entity_Combat combat)
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        anim = GetComponentInChildren<Animator>();

        this.combat = combat;

        direction = (targetPosition - (Vector2)transform.position).normalized;

        rb.linearVelocity = direction * speed;
      
        if (rb.linearVelocity.x < 0)
            transform.Rotate(0, 180, 0);

        Destroy(gameObject, lifeTime);

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        
        if (((1 << collision.gameObject.layer) & whatIsTarget) != 0)
        {
            combat.PerformAttackOnTarget(collision.transform);
            BulletImpact();
        }
    }

    private void BulletImpact()
    {
        
        Destroy(gameObject, 0.05f);
    }
}
