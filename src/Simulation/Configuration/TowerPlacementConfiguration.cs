namespace OpenTD.Simulation.Configuration;

public sealed record TowerPlacementConfiguration(
    int BuildCost,
    float TowerRadius,
    float AttackRange)
{
    public static TowerPlacementConfiguration Default { get; } = new(5, 28, 150);
}
