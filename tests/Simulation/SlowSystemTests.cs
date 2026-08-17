using System.Numerics;
using OpenTD.Simulation.Commands;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Configuration;
using OpenTD.Simulation.Events;
using OpenTD.Simulation.Systems;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class SlowSystemTests
{
    [Fact]
    public void SlowReducesMovementUntilItExpires()
    {
        var world = CreateMovingEntity();
        var entity = Assert.Single(world.Query<Movement>());
        world.Emit(new SlowRequested(entity, 0.5f, 2));
        var slowSystem = new SlowSystem();
        var movementSystem = new MovementSystem();

        slowSystem.Update(world, 0);
        movementSystem.Update(world, 1);
        Assert.Equal(new Vector2(5, 0), world.GetComponent<Position>(entity).Value);
        slowSystem.Update(world, 2);
        movementSystem.Update(world, 1);

        Assert.Equal(new Vector2(15, 0), world.GetComponent<Position>(entity).Value);
        Assert.False(world.TryGetComponent<SlowEffect>(entity, out _));
    }

    [Fact]
    public void StrongerSlowWinsAndDurationIsRefreshed()
    {
        var world = CreateMovingEntity();
        var entity = Assert.Single(world.Query<Movement>());
        world.SetComponent(entity, new SlowEffect(0.7f, 1));
        world.Emit(new SlowRequested(entity, 0.5f, 3));

        new SlowSystem().Update(world, 0);

        Assert.Equal(new SlowEffect(0.5f, 3), world.GetComponent<SlowEffect>(entity));
    }

    [Fact]
    public void SlowingProjectileAppliesEffectOnImpact()
    {
        var world = CreateMovingEntity();
        var enemy = Assert.Single(world.Query<Movement>());
        var projectile = world.CreateEntity();
        world.SetComponent(projectile, world.GetComponent<Position>(enemy));
        world.SetComponent(projectile, new Projectile(enemy, 100));
        world.SetComponent(projectile, new Damage(1));
        world.SetComponent(projectile, new SlowOnHit(0.6f, 2));

        new ProjectileSystem().Update(world, 0);
        new SlowSystem().Update(world, 0);

        Assert.False(world.IsAlive(projectile));
        Assert.Equal(new SlowEffect(0.6f, 2), world.GetComponent<SlowEffect>(enemy));
    }

    [Fact]
    public void SlowingTowerCopiesEffectToProjectile()
    {
        var world = new SimulationWorld();
        var currency = world.CreateEntity();
        world.SetComponent(currency, new Currency(20));
        var map = new MapConfiguration(
            200,
            200,
            5,
            new Vector2[] { new(0, 100), new(200, 100) });
        var placement = new TowerPlacementSystem(
            map,
            TowerPlacementConfiguration.Default);
        world.EnqueueCommand(
            new PlaceTower(new Vector2(50, 50), TowerArchetypeId.Slowing));
        placement.Update(world, 0);
        var tower = Assert.Single(world.Query<Tower>());

        var enemy = world.CreateEntity();
        world.SetComponent(enemy, new Enemy(1));
        world.SetComponent(enemy, new Position(new Vector2(60, 50)));
        world.SetComponent(enemy, new Health(5, 5));
        new TargetingSystem().Update(world, 0);
        new AttackSystem().Update(world, 0);

        var projectile = Assert.Single(world.Query<Projectile>());
        Assert.Equal(
            new SlowOnHit(0.55f, 2.5f),
            world.GetComponent<SlowOnHit>(projectile));
        Assert.Equal(12, world.GetComponent<Currency>(currency).Amount);
        Assert.Equal(TowerArchetypeId.Slowing, world.GetComponent<TowerArchetype>(tower).Id);
    }

    private static SimulationWorld CreateMovingEntity()
    {
        var world = new SimulationWorld();
        var entity = world.CreateEntity();
        world.SetComponent(entity, new Position(Vector2.Zero));
        world.SetComponent(entity, new Movement(10));
        world.SetComponent(
            entity,
            new PathProgress([Vector2.Zero, new Vector2(100, 0)], 1));
        return world;
    }
}
