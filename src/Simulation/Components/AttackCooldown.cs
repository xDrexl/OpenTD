namespace OpenTD.Simulation.Components;

public readonly record struct AttackCooldown(float IntervalSeconds, float RemainingSeconds);
