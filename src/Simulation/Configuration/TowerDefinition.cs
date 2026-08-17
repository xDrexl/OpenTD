using OpenTD.Simulation.Components;

namespace OpenTD.Simulation.Configuration;

public sealed record TowerDefinition(
    TowerArchetypeId Archetype,
    int BuildCost,
    float PlacementRadius,
    float AttackRange,
    float AttackIntervalSeconds,
    int AttackDamage,
    float ProjectileSpeed);
