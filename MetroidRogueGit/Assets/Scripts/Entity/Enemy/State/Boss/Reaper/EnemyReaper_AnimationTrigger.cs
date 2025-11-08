using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;

public class EnemyReaper_AnimationTrigger : Enemy_AnimationTrigger
{
    private Enemy_Reaper enemyReaper;


    protected override void Awake()
    {
        base.Awake();
        enemyReaper = GetComponentInParent<Enemy_Reaper>();
        //bossCombat = GetComponentInParent<EnemyBoss_Combat>();
    }

    private void TeleportTrigger()
    {
        enemyReaper.SetTeleportTrigger(true);
    }

    private void MagicMeleeTajo()
    {
        if (entityCombat is EnemyBoss_Combat bossCombat)
            bossCombat.PerformAttackType(BossAttackType.MagicMelee);

        enemyReaper.CreateMagicTajo();
    }

    protected override void AttackTrigger()
    {
        if (entityCombat is EnemyBoss_Combat bossCombat)
            bossCombat.PerformAttackType(BossAttackType.Melee);
       
    }



}
