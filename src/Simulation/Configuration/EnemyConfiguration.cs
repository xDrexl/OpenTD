using OpenTD.Simulation.Components;

namespace OpenTD.Simulation.Configuration;

public sealed record EnemyConfiguration(
    float Speed,
    int Health,
    int BaseDamage,
    int Reward,
    EnemyArchetypeId Archetype = EnemyArchetypeId.Basic);
