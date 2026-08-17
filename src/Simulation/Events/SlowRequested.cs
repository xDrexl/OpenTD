using OpenTD.Simulation.World;

namespace OpenTD.Simulation.Events;

public readonly record struct SlowRequested(
    Entity Target,
    float SpeedMultiplier,
    float DurationSeconds);
