using UnityEngine;

public class Entity_SetIDamagle : MonoBehaviour
{
    protected IDamageable iDamagable;
    private void OnCollisionEnter2D(Collision2D collision)
    {
        var targetCollider = collision.collider;
        iDamagable = targetCollider.GetComponentInChildren<IDamageable>();
    }
}
