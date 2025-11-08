using UnityEngine;

public class Enemy_AnimationTrigger : Entity_AnimationTriggers
{
    private Enemy enemy;
    private Enemy_VFX enemyVFX;

    protected override void Awake()
    {
        base.Awake();
        enemy = GetComponentInParent<Enemy>();
        enemyVFX = GetComponentInParent<Enemy_VFX>();

    }
    private void SpecialAttackTrigger()
    {
        enemy.SpecialAttack();
    }


    void EnableCouterWindow()
    {
        //enemyVFX.EnableAttackAlert(true);
        enemy.EnableCounter(true);
    }

    void EnablePerfectParryWindow()
    {
        enemyVFX.EnableAttackAlert(true);
        enemy.EnablePerfectCounter(true);
    }

    void DisableCounterWindow()
    {
        //enemyVFX.EnableAttackAlert(false);
        enemy.EnableCounter(false);
        
    }
    void DisablePerfectParryWindow()
    {
        enemyVFX.EnableAttackAlert(false);
        enemy.EnablePerfectCounter(false);
    }


}
