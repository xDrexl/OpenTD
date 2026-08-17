using System.Numerics;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Systems;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class TargetingSystemTests
{
    [Fact]
    public void TowerTargetsNearestEnemyInRange()
    {
        var world = new SimulationWorld();
        var tower = CreateTower(world, Vector2.Zero, 10);
        CreateEnemy(world, new Vector2(8, 0));
        var nearestEnemy = CreateEnemy(world, new Vector2(3, 0));

        new TargetingSystem().Update(world, 0);

        Assert.Equal(nearestEnemy, world.GetComponent<Target>(tower).Entity);
    }

    [Fact]
    public void EnemyAtRangeBoundaryIsTargetable()
    {
        var world = new SimulationWorld();
        var tower = CreateTower(world, Vector2.Zero, 5);
        var enemy = CreateEnemy(world, new Vector2(3, 4));

        new TargetingSystem().Update(world, 0);

        Assert.Equal(enemy, world.GetComponent<Target>(tower).Entity);
    }

    [Fact]
    public void TowerLosesTargetWhenEnemyLeavesRange()
    {
        var world = new SimulationWorld();
        var tower = CreateTower(world, Vector2.Zero, 5);
        var enemy = CreateEnemy(world, new Vector2(2, 0));
        var system = new TargetingSystem();
        system.Update(world, 0);
        world.SetComponent(enemy, new Position(new Vector2(6, 0)));

        system.Update(world, 0);

        Assert.False(world.TryGetComponent<Target>(tower, out _));
    }

    [Fact]
    public void TowerRetargetsWhenCurrentEnemyIsRemoved()
    {
        var world = new SimulationWorld();
        var tower = CreateTower(world, Vector2.Zero, 10);
        var firstEnemy = CreateEnemy(world, new Vector2(2, 0));
        var secondEnemy = CreateEnemy(world, new Vector2(4, 0));
        var system = new TargetingSystem();
        system.Update(world, 0);
        world.DestroyEntity(firstEnemy);

        system.Update(world, 0);

        Assert.Equal(secondEnemy, world.GetComponent<Target>(tower).Entity);
    }

    [Fact]
    public void CompletedEnemyIsNotTargetable()
    {
        var world = new SimulationWorld();
        var tower = CreateTower(world, Vector2.Zero, 10);
        var enemy = CreateEnemy(world, new Vector2(2, 0));
        world.SetComponent(enemy, new PathProgress([Vector2.Zero], 1, true));

        new TargetingSystem().Update(world, 0);

        Assert.False(world.TryGetComponent<Target>(tower, out _));
    }

    [Fact]
    public void EqualDistanceTargetsLowestEntityIdDeterministically()
    {
        var world = new SimulationWorld();
        var tower = CreateTower(world, Vector2.Zero, 10);
        var firstEnemy = CreateEnemy(world, new Vector2(-2, 0));
        CreateEnemy(world, new Vector2(2, 0));

        new TargetingSystem().Update(world, 0);

        Assert.Equal(firstEnemy, world.GetComponent<Target>(tower).Entity);
    }

    private static Entity CreateTower(SimulationWorld world, Vector2 position, float range)
    {
        var tower = world.CreateEntity();
        world.SetComponent(tower, new Tower());
        world.SetComponent(tower, new Position(position));
        world.SetComponent(tower, new AttackRange(range));
        return tower;
    }

    private static Entity CreateEnemy(SimulationWorld world, Vector2 position)
    {
        var enemy = world.CreateEntity();
        world.SetComponent(enemy, new Enemy(1));
        world.SetComponent(enemy, new Position(position));
        return enemy;
    }
}
