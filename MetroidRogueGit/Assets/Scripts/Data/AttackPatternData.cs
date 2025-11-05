using System.Collections.Generic;
using System;

[Serializable]
public class AttackPatternData
{
    public string patternName;
    public BossPhase phase; // Para qué fase aplica este patrón
    public List<AttackTiming> attacks;
}

[Serializable]
public class AttackTiming
{
    public BossAttackType type;
    public float postDelay; // Tiempo de espera tras ejecutar ese ataque
}
