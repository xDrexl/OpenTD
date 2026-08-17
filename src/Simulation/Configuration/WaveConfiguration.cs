using System.Collections.Generic;
using System.Numerics;

namespace OpenTD.Simulation.Configuration;

public sealed record WaveConfiguration(
    IReadOnlyList<WaveDefinition> Waves,
    float InterWaveDelaySeconds,
    IReadOnlyList<Vector2> Path)
{
    public static WaveConfiguration CreateDefault(MapConfiguration map) => new(
        new WaveDefinition[]
        {
            new(3, 1.5f, new EnemyConfiguration(100, 10, 1, 3)),
            new(5, 1.2f, new EnemyConfiguration(110, 12, 1, 3)),
            new(7, 1, new EnemyConfiguration(120, 14, 1, 4)),
        },
        InterWaveDelaySeconds: 3,
        Path: map.Path);
}
