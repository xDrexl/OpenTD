using System.Linq;
using System.Numerics;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Configuration;
using OpenTD.Simulation.Systems;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class ProceduralMapGeneratorTests
{
    private readonly ProceduralMapGenerator _generator = new();

    [Fact]
    public void SameRunSeedAndStageProduceSameMap()
    {
        var first = _generator.Generate(123456789, 7);
        var second = _generator.Generate(123456789, 7);

        Assert.Equal(first.TerrainSeed, second.TerrainSeed);
        Assert.Equal(first.Path, second.Path);
        Assert.Equal(first.BuildZones, second.BuildZones);
        Assert.Equal(first.Obstacles, second.Obstacles);
    }

    [Fact]
    public void DifferentStagesProduceDifferentMaps()
    {
        var first = _generator.Generate(123456789, 1);
        var second = _generator.Generate(123456789, 2);

        Assert.True(
            first.TerrainSeed != second.TerrainSeed ||
            !first.Path.SequenceEqual(second.Path) ||
            !first.Obstacles!.SequenceEqual(second.Obstacles!));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(100)]
    public void GeneratedMapStaysWithinBounds(int stageNumber)
    {
        var map = _generator.Generate(-8675309, stageNumber);

        Assert.All(map.Path, point =>
        {
            Assert.InRange(point.X, 0, map.Width);
            Assert.InRange(point.Y, 0, map.Height);
        });
        Assert.Equal(70, map.Path[0].X);
        Assert.Equal(1070, map.Path[^1].X);
        Assert.All(map.Obstacles!, obstacle =>
        {
            Assert.True(obstacle.Minimum.X >= 48);
            Assert.True(obstacle.Minimum.Y >= 48);
            Assert.True(obstacle.Maximum.X <= map.Width - 48);
            Assert.True(obstacle.Maximum.Y <= map.Height - 48);
        });
    }

    [Theory]
    [InlineData(1)]
    [InlineData(40)]
    [InlineData(80)]
    public void ObstaclesStayClearOfPathSpawnAndBase(int stageNumber)
    {
        var map = _generator.Generate(42, stageNumber);
        const float clearance = ProceduralMapGenerator.PathHalfWidth + 16;

        foreach (var obstacle in map.Obstacles!)
        {
            var expandedMinimum = obstacle.Minimum - new Vector2(clearance);
            var expandedMaximum = obstacle.Maximum + new Vector2(clearance);
            for (var index = 0; index < map.Path.Count - 1; index++)
            {
                var segmentMinimum = Vector2.Min(map.Path[index], map.Path[index + 1]);
                var segmentMaximum = Vector2.Max(map.Path[index], map.Path[index + 1]);
                Assert.False(
                    segmentMaximum.X >= expandedMinimum.X &&
                    segmentMinimum.X <= expandedMaximum.X &&
                    segmentMaximum.Y >= expandedMinimum.Y &&
                    segmentMinimum.Y <= expandedMaximum.Y);
            }
        }
    }

    [Theory]
    [InlineData(1)]
    [InlineData(20)]
    [InlineData(60)]
    public void GeneratedMapHasUsableBuildSpace(int stageNumber)
    {
        var map = _generator.Generate(314159, stageNumber);
        var world = new SimulationWorld();
        var currencyEntity = world.CreateEntity();
        world.SetComponent(currencyEntity, new Currency(999));
        var placement = new TowerPlacementSystem(
            map,
            TowerPlacementConfiguration.Default);

        var usablePositions = 0;
        for (var y = 64; y < map.Height - 64; y += 64)
        {
            for (var x = 64; x < map.Width - 64; x += 64)
            {
                if (placement.CanPlace(world, new Vector2(x, y)))
                {
                    usablePositions++;
                }
            }
        }

        Assert.True(usablePositions >= 10);
    }
}
