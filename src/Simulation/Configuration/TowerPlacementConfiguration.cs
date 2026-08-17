namespace OpenTD.Simulation.Configuration;

public sealed record TowerPlacementConfiguration(
    int BuildCost,
    float TowerRadius,
    float AttackRange,
    float AttackIntervalSeconds,
    int AttackDamage,
    float ProjectileSpeed)
{
    public static TowerPlacementConfiguration Default { get; } = new(
        5,
        28,
        150,
        0.75f,
        2,
        250);
}
