using System;
using System.Numerics;

namespace OpenTD.Simulation.Configuration;

public readonly record struct MapRegion(Vector2 Minimum, Vector2 Maximum)
{
    public bool ContainsCircle(Vector2 center, float radius) =>
        center.X - radius >= Minimum.X &&
        center.Y - radius >= Minimum.Y &&
        center.X + radius <= Maximum.X &&
        center.Y + radius <= Maximum.Y;

    public bool IntersectsCircle(Vector2 center, float radius)
    {
        var closestX = Math.Clamp(center.X, Minimum.X, Maximum.X);
        var closestY = Math.Clamp(center.Y, Minimum.Y, Maximum.Y);
        return Vector2.DistanceSquared(center, new Vector2(closestX, closestY)) <
               radius * radius;
    }
}
