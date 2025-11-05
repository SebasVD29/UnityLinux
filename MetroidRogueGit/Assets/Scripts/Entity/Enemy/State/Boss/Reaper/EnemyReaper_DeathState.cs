using System.Collections;
using UnityEngine;


public class EnemyReaper_DeathState : EnemyState
{
    private Enemy_Reaper enemyReaper;
    private BossManager bossManager;
    //private Collider2D collider;
    //private bool isDeadHandled = false;
    public EnemyReaper_DeathState(Enemy enemy, StateMachine stateMachine, string animBoolName) : base(enemy, stateMachine, animBoolName)
    {
        enemyReaper = enemy as Enemy_Reaper;
        bossManager = GameObject.FindFirstObjectByType<BossManager>();
        //collider = enemyReaper.GetComponent<Collider2D>();
    }
    public override void Enter()
    {
        Debug.Log("Reaper DEATH " + animBoolName);
        anim.SetBool(animBoolName, true);
        stateMachine.SwitchOffStateMachine();
        enemyReaper.SetVelocity(0, 0);
        enemyReaper.StartCoroutine(HandleDeath());
    }

    private IEnumerator HandleDeath()
    {
        //Debug.Log("Reaper DEATH Coroutine " + animBoolName);
        yield return new WaitForSeconds(0.8f); // o anim.GetCurrentAnimatorStateInfo(0).length
        bossManager.BossDeath();
        // Esperar a que termine la animación o un tiempo fijo
        yield return new WaitForSeconds(1f); // o anim.GetCurrentAnimatorStateInfo(0).length

      
    }

   

}
