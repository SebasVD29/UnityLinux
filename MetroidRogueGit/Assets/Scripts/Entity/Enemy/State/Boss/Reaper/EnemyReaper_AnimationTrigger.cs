using UnityEngine;

public class EnemyReaper_AnimationTrigger : Enemy_AnimationTrigger
{
    private Enemy_Reaper enemyReaper;

    protected override void Awake()
    {
        base.Awake();
        enemyReaper = GetComponentInParent<Enemy_Reaper>();
    }

    private void TeleportTrigger()
    {
        enemyReaper.SetTeleportTrigger(true);
    }

    private void MagicMeleeTajo()
    {
        enemyReaper.CreateMagicTajo();
    }

}
