using System.Collections.Generic;
using System.Numerics;

namespace OpenTD.Simulation.Configuration;

public sealed record MapConfiguration(
    float Width,
    float Height,
    float PathHalfWidth,
    IReadOnlyList<Vector2> Path)
{
    public static MapConfiguration CreateDefault() => new(
        1152,
        648,
        36,
        new Vector2[]
        {
            new(70, 324),
            new(224, 324),
            new(224, 176),
            new(480, 176),
            new(480, 456),
            new(736, 456),
            new(736, 192),
            new(1070, 192),
        });
}
