using UnityEngine;

public class Skill_Parry : Skill_Base
{
   
    protected override void Awake()
    {
        base.Awake();
        
        
    }



    public void Parry(Enemy enemy,bool parry, bool isPerfect)
    {

        if (Unlocked(SkillUnlockType.None))
        {
            Debug.Log("No valied parry unlock selected!");
            return;
        }

        SetSkillOnCooldown();

        if (Unlocked(SkillUnlockType.PerfectParry) && isPerfect == true)
            InstaKillPerfectParry(enemy, parry);

        if (Unlocked(SkillUnlockType.StealParry))
            StealHealthParry(enemy);

    }

    

    private void InstaKillPerfectParry(Enemy enemy, bool parry)
    {
        if (enemy == null)
            return;

        Debug.Log("Skill Perfect Parry");
        enemy.ReduceParryCounts(parry);

    }

    private void StealHealthParry(Enemy enemy)
    {
        if (enemy == null || player == null)
            return;
        
           
        Debug.Log("Skill Steal Parry");
        vfx?.CreateOnHealthVFX(player.transform);

        enemy.StealHealthToPlayer(player);
    }
}
