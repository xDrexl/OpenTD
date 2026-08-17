using System;
using System.Collections.Generic;
using System.Numerics;

namespace OpenTD.Simulation.Configuration;

public sealed class ProceduralMapGenerator
{
    public const float MapWidth = 1152;
    public const float MapHeight = 648;
    public const float PathHalfWidth = 36;

    private const float SpawnX = 70;
    private const float BaseX = 1070;
    private const float BoundaryClearance = 48;
    private const float ObstacleSize = 72;
    private const float ObstaclePathClearance = 16;
    private const int ObstacleCount = 3;

    public MapConfiguration Generate(long runSeed, int stageNumber)
    {
        if (stageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(stageNumber));
        }

        var random = new DeterministicRandom(
            unchecked((ulong)runSeed ^ ((ulong)stageNumber * 0x9E3779B97F4A7C15UL)));
        var path = GeneratePath(ref random);
        var obstacles = GenerateObstacles(path, ref random);

        return new MapConfiguration(
            MapWidth,
            MapHeight,
            PathHalfWidth,
            path,
            BuildZones:
            [
                new MapRegion(
                    new Vector2(28, 28),
                    new Vector2(MapWidth - 28, MapHeight - 28)),
            ],
            Obstacles: obstacles,
            TerrainSeed: random.NextInt(int.MaxValue));
    }

    private static IReadOnlyList<Vector2> GeneratePath(ref DeterministicRandom random)
    {
        var xCoordinates = new[] { SpawnX, 270, 470, 670, 870, BaseX };
        var yCoordinates = new float[xCoordinates.Length];
        for (var index = 0; index < yCoordinates.Length; index++)
        {
            yCoordinates[index] = 120 + random.NextInt(7) * 68;
        }

        var path = new List<Vector2> { new(xCoordinates[0], yCoordinates[0]) };
        for (var index = 1; index < xCoordinates.Length; index++)
        {
            path.Add(new Vector2(xCoordinates[index], yCoordinates[index - 1]));
            if (yCoordinates[index] != yCoordinates[index - 1])
            {
                path.Add(new Vector2(xCoordinates[index], yCoordinates[index]));
            }
        }

        return path;
    }

    private static IReadOnlyList<MapRegion> GenerateObstacles(
        IReadOnlyList<Vector2> path,
        ref DeterministicRandom random)
    {
        var obstacles = new List<MapRegion>();
        for (var attempt = 0; attempt < 500 && obstacles.Count < ObstacleCount; attempt++)
        {
            var center = new Vector2(
                BoundaryClearance + ObstacleSize / 2 +
                random.NextInt((int)(MapWidth - 2 * BoundaryClearance - ObstacleSize)),
                BoundaryClearance + ObstacleSize / 2 +
                random.NextInt((int)(MapHeight - 2 * BoundaryClearance - ObstacleSize)));
            var halfSize = new Vector2(ObstacleSize / 2);
            var candidate = new MapRegion(center - halfSize, center + halfSize);

            if (IntersectsPath(candidate, path, PathHalfWidth + ObstaclePathClearance) ||
                obstacles.Exists(obstacle => RegionsOverlap(candidate, obstacle, 20)))
            {
                continue;
            }

            obstacles.Add(candidate);
        }

        if (obstacles.Count != ObstacleCount)
        {
            throw new InvalidOperationException("Unable to generate a valid obstacle layout.");
        }

        return obstacles;
    }

    private static bool IntersectsPath(
        MapRegion region,
        IReadOnlyList<Vector2> path,
        float clearance)
    {
        var expanded = new MapRegion(
            region.Minimum - new Vector2(clearance),
            region.Maximum + new Vector2(clearance));

        for (var index = 0; index < path.Count - 1; index++)
        {
            var start = path[index];
            var end = path[index + 1];
            var segmentMinimum = Vector2.Min(start, end);
            var segmentMaximum = Vector2.Max(start, end);
            if (segmentMaximum.X >= expanded.Minimum.X &&
                segmentMinimum.X <= expanded.Maximum.X &&
                segmentMaximum.Y >= expanded.Minimum.Y &&
                segmentMinimum.Y <= expanded.Maximum.Y)
            {
                return true;
            }
        }

        return false;
    }

    private static bool RegionsOverlap(MapRegion left, MapRegion right, float clearance) =>
        left.Minimum.X < right.Maximum.X + clearance &&
        left.Maximum.X > right.Minimum.X - clearance &&
        left.Minimum.Y < right.Maximum.Y + clearance &&
        left.Maximum.Y > right.Minimum.Y - clearance;

    private struct DeterministicRandom(ulong state)
    {
        private ulong _state = state;

        public int NextInt(int exclusiveMaximum)
        {
            if (exclusiveMaximum <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(exclusiveMaximum));
            }

            _state += 0x9E3779B97F4A7C15UL;
            var value = _state;
            value = (value ^ (value >> 30)) * 0xBF58476D1CE4E5B9UL;
            value = (value ^ (value >> 27)) * 0x94D049BB133111EBUL;
            value ^= value >> 31;
            return (int)(value % (uint)exclusiveMaximum);
        }
    }
}
