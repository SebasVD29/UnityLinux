using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyBoss : Enemy, ICounterable
{
    public BossPhase currentPhase { get; private set; } = BossPhase.Phase1;

    [Header("Boss Time btw Attack")]
    public float nextDecisionTime;

    [Header("Boss Arena")]
    [SerializeField] protected BoxCollider2D arenaBounds;

    [Header("Attack Pattern Settings")]
    private BossPhase loadedPatternPhase;
    public List<AttackPatternData> attackPatterns = new List<AttackPatternData>(); // Configurable en el inspector
    private Queue<AttackTiming> currentPatternQueue = new Queue<AttackTiming>();

    public bool CanBeParry { get => canBeStunned; }
    public bool CanBePerfectParry { get => canBePerfect; }

    public virtual void UpdatePhase()
    {
        float healthPercent = health.GetHealthPercent();

        var oldPhase = currentPhase;

        if (healthPercent <= 0.33f)
            currentPhase = BossPhase.Phase3;
        else if (healthPercent <= 0.66f)
            currentPhase = BossPhase.Phase2;
        else
            currentPhase = BossPhase.Phase1;

        if (oldPhase != currentPhase)
            ApplyPhaseModifiers();
    }
    public virtual void ApplyPhaseModifiers()
    {
     
    }

    public bool HasPatternLoadedForPhase(BossPhase phase) => loadedPatternPhase == phase;

    public virtual void LoadAttackPatternForPhase(BossPhase phase)
    {
        currentPatternQueue.Clear();
        var availablePatterns = attackPatterns.Where(p => p.phase == phase).ToList();

        if (availablePatterns == null || availablePatterns.Count == 0)
        {
            Debug.LogWarning($"No pattern found for phase {phase}, using fallback Melee only.");
            currentPatternQueue.Enqueue(new AttackTiming { type = BossAttackType.Melee, postDelay = 0.5f });
            loadedPatternPhase = phase;
            return;
        }

        var selectedPattern = availablePatterns[Random.Range(0, availablePatterns.Count)];

        foreach (var atk in selectedPattern.attacks)
            currentPatternQueue.Enqueue(atk);

        loadedPatternPhase = phase;
    }

    public virtual AttackTiming GetNextAttackFromPattern()
    {
        if (currentPatternQueue.Count == 0)
            LoadAttackPatternForPhase(currentPhase);

        return currentPatternQueue.Dequeue();
    }

    public virtual AttackTiming PeekNextAttackFromPattern()
    {
        if (currentPatternQueue.Count == 0)
            LoadAttackPatternForPhase(currentPhase);

        return currentPatternQueue.Peek();
    }

    public virtual AttackTiming DequeueNextAttackFromPattern()
    {
        if (currentPatternQueue.Count == 0)
            LoadAttackPatternForPhase(currentPhase);

        return currentPatternQueue.Dequeue();
    }

    public void IntroBoss(bool onCamera)
    {
        if (onCamera == true)
            anim.SetBool("Intro", onCamera);
        else
        {
            anim.SetBool("Intro", onCamera);
            stateMachine.Initialize(idleState);
        }

    }

    public void HandleCounter()
    {
        if (CanBeParry == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }

    public void HandlePerfectCounter()
    {
        if (CanBePerfectParry == false)
            return;

        stateMachine.ChangeState(stunnedState);
    }
}
public enum BossPhase { Phase1, Phase2, Phase3 }
public enum TeleportType { Random, Left, Right, Center }