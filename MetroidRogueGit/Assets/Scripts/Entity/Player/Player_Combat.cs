using Unity.VisualScripting;
using UnityEngine;

public class Player_Combat : Entity_Combat
{
    Player player;

    [Header("Parry Attack")]
    [SerializeField] private float parryRecovery = .1f;
    [SerializeField] private LayerMask whatIsParry;

    [Header("Parry Target")]
    [SerializeField] private Transform targetParryCheck;
    [SerializeField] private float targetParryCheckRadius = 1;
 
    protected override void Awake()
    {
        base.Awake();

       player = GetComponent<Player>();
    }
    public bool ParryAttack(out bool isPerfect)
    {
        bool hasParryPerformed = false;
        isPerfect = false;

        foreach (var target in GetDetectedToParryColliders(whatIsParry))
        {
            ICounterable counterable = target.GetComponent<ICounterable>();
            Enemy enemy = target.GetComponent<Enemy>();

            //Debug.Log("Objetc " + counterable.CanBeParry + " " + counterable.CanBePerfectParry);

            if (counterable == null || enemy == null)
                continue;

            if (counterable.CanBeParry)
            {
                counterable.HandleCounter();
                hasParryPerformed = true;
                isPerfect = false;
                if (player.skillManager.parry.CanUseSkill())
                {
                    player.skillManager.parry.Parry(enemy, hasParryPerformed, isPerfect);
                }
                vfx.CreateOnHitVFX(target.transform, Color.white);
            }

            if (counterable.CanBePerfectParry)
            { 
                counterable.HandlePerfectCounter();
                hasParryPerformed = true;
                isPerfect = true;
                if (player.skillManager.parry.CanUseSkill())
                {
                    player.skillManager.parry.Parry(enemy, hasParryPerformed, isPerfect);
                }
                vfx.CreateOnHitVFX(target.transform, Color.blue);      
            }

            
        }
        
        return hasParryPerformed;
    }

    protected Collider2D[] GetDetectedToParryColliders(LayerMask whatToDetect)
    {
         return Physics2D.OverlapCircleAll(targetParryCheck.position, targetParryCheckRadius, whatToDetect);
    }
    public override void OnDrawGizmos()
    {
        base.OnDrawGizmos();

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(targetParryCheck.position, targetParryCheckRadius);
    }

    public float GetParryRecovery() => parryRecovery;
}
