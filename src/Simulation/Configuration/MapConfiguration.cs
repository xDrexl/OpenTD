using System.Collections.Generic;
using System.Numerics;

namespace OpenTD.Simulation.Configuration;

public sealed record MapConfiguration(
    float Width,
    float Height,
    float PathHalfWidth,
    IReadOnlyList<Vector2> Path,
    IReadOnlyList<MapRegion>? BuildZones = null,
    IReadOnlyList<MapRegion>? Obstacles = null,
    int TerrainSeed = 0)
{
    public static MapConfiguration CreateDefault() => new(
        1152,
        648,
        36,
        Path: new Vector2[]
        {
            new(70, 324),
            new(224, 324),
            new(224, 176),
            new(480, 176),
            new(480, 456),
            new(736, 456),
            new(736, 192),
            new(1070, 192),
        },
        BuildZones:
        [
            new MapRegion(new Vector2(28, 28), new Vector2(1124, 620)),
        ],
        Obstacles:
        [
            new MapRegion(new Vector2(300, 250), new Vector2(390, 330)),
            new MapRegion(new Vector2(560, 50), new Vector2(650, 130)),
            new MapRegion(new Vector2(820, 340), new Vector2(920, 440)),
        ]);
}
