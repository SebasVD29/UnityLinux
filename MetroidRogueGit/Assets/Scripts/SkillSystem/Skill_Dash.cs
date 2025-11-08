
using System.Collections;
using System.Collections.Generic;
using System.Drawing;

using UnityEngine;


public class Skill_Dash : Skill_Base
{
   

    [Header("Target detection")]
    [SerializeField] private Transform targetCheck;
    [SerializeField] private Vector2 targetSize = new Vector2(2f, 4f);
    [SerializeField] private float angle = 1;
    [SerializeField]private LayerMask whatIsTarget;

    private HashSet<GameObject> dashHitEnemies;

    private Coroutine darkDashCoroutine;
    private Coroutine dashAttackCoroutine;
    public void Dash()
    {
        //if (Unlocked(SkillUnlockType.None))
        //{
        //    Debug.Log("No valied dash unlock selected!");
        //    return;
        //}

        if (Unlocked(SkillUnlockType.DashAttack))
            PlayDashAttack();

        if (Unlocked(SkillUnlockType.DarkDash))
            PlayDarkDash();
    }
 
    public void PlayDashAttack()
    {
        Debug.Log("DashAttack");
        if (dashAttackCoroutine != null)
            StopCoroutine(dashAttackCoroutine);

        dashAttackCoroutine = StartCoroutine(DashAttackCo());
    }
    private IEnumerator DashAttackCo()
    {

        dashHitEnemies = new HashSet<GameObject>();

        Physics2D.IgnoreLayerCollision(6, 9, true);

        float dashTime = player.dashDuration;
        float elapsed = 0f;

        while (elapsed < dashTime)
        {
            PerformDashAttack();
            elapsed += Time.deltaTime;
            yield return null;
        }

        Physics2D.IgnoreLayerCollision(6, 9, false);

        dashHitEnemies.Clear();
    }

    void PerformDashAttack()
    {

        foreach (var target in GetDetectedColliders())
        {
            if (dashHitEnemies.Contains(target.gameObject))
                continue; // ya fue golpeado este dash

            IDamageable damagable = target.GetComponent<IDamageable>();
            if (damagable == null)
                continue;

            float elemental = player.stats.GetElementalDamage(out ElementType element, .6f);
            float damage = player.stats.GetPhyiscalDamage(out bool isCrit);
            bool targetGotHit = damagable.TakeDamage(damage, elemental, transform, element);

            if (targetGotHit)
            {
                vfx.CreateOnHitVFX(target.transform, UnityEngine.Color.red);
                dashHitEnemies.Add(target.gameObject); // marcar como golpeado
            }
        }
    }

    public void PlayDarkDash()
    {
        Debug.Log("DarkDash");
        if (darkDashCoroutine != null)
            StopCoroutine(darkDashCoroutine);

        darkDashCoroutine = StartCoroutine(DarkDashCo());
    }
    private IEnumerator DarkDashCo()
    {

        Physics2D.IgnoreLayerCollision(6, 9, true);
        yield return new WaitForSeconds(player.dashDuration);
        Physics2D.IgnoreLayerCollision(6, 9, false);
    }

    protected Collider2D[] GetDetectedColliders()
    {
        return Physics2D.OverlapCapsuleAll(targetCheck.position, targetSize, CapsuleDirection2D.Vertical, angle,whatIsTarget);
    }

    private void OnDrawGizmos()
    {
        //Gizmos.DrawWireSphere(targetCheck.position, targetSize);
        Gizmos.DrawWireCube(targetCheck.position, new Vector3(targetSize.x, targetSize.y, 0));
    }
}
