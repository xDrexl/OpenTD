using System.Numerics;
using OpenTD.Simulation.Commands;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Configuration;
using OpenTD.Simulation.Systems;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class TowerVarietyIntegrationTests
{
    [Fact]
    public void RapidTowerUsesExistingTargetingAndAttackSystemsUnchanged()
    {
        var world = new SimulationWorld();
        var currency = world.CreateEntity();
        world.SetComponent(currency, new Currency(20));
        var map = new MapConfiguration(
            200,
            200,
            5,
            new Vector2[] { new(0, 100), new(200, 100) });
        var definition = new TowerDefinition(
            TowerArchetypeId.Rapid,
            7,
            8,
            40,
            0.25f,
            1,
            150);
        var placement = new TowerPlacementSystem(
            map,
            new TowerPlacementConfiguration([definition]));
        world.EnqueueCommand(new PlaceTower(new Vector2(50, 50), TowerArchetypeId.Rapid));
        placement.Update(world, 0);

        var tower = Assert.Single(world.Query<Tower>());
        var enemy = world.CreateEntity();
        world.SetComponent(enemy, new Enemy(1));
        world.SetComponent(enemy, new Position(new Vector2(60, 50)));
        world.SetComponent(enemy, new Health(5, 5));
        new TargetingSystem().Update(world, 0);
        new AttackSystem().Update(world, 0);

        Assert.Equal(TowerArchetypeId.Rapid, world.GetComponent<TowerArchetype>(tower).Id);
        Assert.Equal(enemy, world.GetComponent<Target>(tower).Entity);
        Assert.Equal(13, world.GetComponent<Currency>(currency).Amount);
        Assert.Equal(0.25f, world.GetComponent<AttackCooldown>(tower).RemainingSeconds);
        var projectile = Assert.Single(world.Query<Projectile>());
        Assert.Equal(150, world.GetComponent<Projectile>(projectile).Speed);
        Assert.Equal(1, world.GetComponent<Damage>(projectile).Amount);
    }
}
