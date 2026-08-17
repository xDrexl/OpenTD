namespace OpenTD.Simulation.Components;

public enum EnemyArchetypeId
{
    Basic,
    Fast,
}

public readonly record struct EnemyArchetype(EnemyArchetypeId Id);
