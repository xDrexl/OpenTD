using System.Numerics;
using OpenTD.Simulation.Commands;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Configuration;
using OpenTD.Simulation.Systems;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class TowerPlacementSystemTests
{
    private static readonly MapConfiguration Map = new(
        200,
        200,
        10,
        new Vector2[] { new(0, 100), new(200, 100) });

    private static readonly TowerPlacementConfiguration Configuration = new(
        new TowerDefinition[]
        {
            new(TowerArchetypeId.Basic, 5, 10, 50, 1, 2, 100),
            new(TowerArchetypeId.Rapid, 7, 8, 40, 0.25f, 1, 150),
        });

    [Fact]
    public void ValidPlacementCreatesTowerAndDeductsCurrency()
    {
        var world = CreateWorld(10);
        var system = new TowerPlacementSystem(Map, Configuration);
        world.EnqueueCommand(new PlaceTower(new Vector2(50, 50)));

        system.Update(world, 0);

        var tower = Assert.Single(world.Query<Tower, Position>());
        Assert.Equal(new Vector2(50, 50), world.GetComponent<Position>(tower).Value);
        Assert.Equal(TowerArchetypeId.Basic, world.GetComponent<TowerArchetype>(tower).Id);
        Assert.Equal(10, world.GetComponent<PlacementRadius>(tower).Value);
        Assert.Equal(5, world.GetComponent<BuildCost>(tower).Amount);
        Assert.Equal(50, world.GetComponent<AttackRange>(tower).Radius);
        Assert.Equal(1, world.GetComponent<AttackCooldown>(tower).IntervalSeconds);
        Assert.Equal(new AttackStats(2, 100), world.GetComponent<AttackStats>(tower));
        var currency = Assert.Single(world.Query<Currency>());
        Assert.Equal(5, world.GetComponent<Currency>(currency).Amount);
    }

    [Theory]
    [InlineData(5, 100)]
    [InlineData(50, 115)]
    [InlineData(195, 50)]
    public void InvalidMapPlacementIsRejected(float x, float y)
    {
        var world = CreateWorld(10);
        var system = new TowerPlacementSystem(Map, Configuration);
        world.EnqueueCommand(new PlaceTower(new Vector2(x, y)));

        system.Update(world, 0);

        Assert.Empty(world.Query<Tower>());
        var currency = Assert.Single(world.Query<Currency>());
        Assert.Equal(10, world.GetComponent<Currency>(currency).Amount);
    }

    [Fact]
    public void PlacementIsRejectedWithoutEnoughCurrency()
    {
        var world = CreateWorld(4);
        var system = new TowerPlacementSystem(Map, Configuration);
        world.EnqueueCommand(new PlaceTower(new Vector2(50, 50)));

        system.Update(world, 0);

        Assert.Empty(world.Query<Tower>());
    }

    [Fact]
    public void PlacementCannotOverlapAnotherTower()
    {
        var world = CreateWorld(10);
        var existingTower = world.CreateEntity();
        world.SetComponent(existingTower, new Tower());
        world.SetComponent(existingTower, new Position(new Vector2(50, 50)));
        world.SetComponent(existingTower, new PlacementRadius(10));
        var system = new TowerPlacementSystem(Map, Configuration);
        world.EnqueueCommand(new PlaceTower(new Vector2(60, 50)));

        system.Update(world, 0);

        Assert.Single(world.Query<Tower>());
    }

    private static SimulationWorld CreateWorld(int currencyAmount)
    {
        var world = new SimulationWorld();
        var currency = world.CreateEntity();
        world.SetComponent(currency, new Currency(currencyAmount));
        return world;
    }
}
