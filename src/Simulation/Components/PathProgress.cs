using System.Collections.Generic;
using System.Numerics;

namespace OpenTD.Simulation.Components;

public readonly record struct PathProgress(
    IReadOnlyList<Vector2> Waypoints,
    int NextWaypointIndex,
    bool IsCompleted = false);
