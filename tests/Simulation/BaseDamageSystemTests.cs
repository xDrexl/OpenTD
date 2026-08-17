using System.Numerics;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Events;
using OpenTD.Simulation.Systems;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class BaseDamageSystemTests
{
    [Fact]
    public void CompletingPathEmitsArrivalOnce()
    {
        var world = new SimulationWorld();
        var enemy = CreateEnemy(world, 3);
        world.SetComponent(enemy, new PathProgress([Vector2.Zero], 1));
        var system = new PathCompletionSystem();

        system.Update(world, 0);
        system.Update(world, 0);

        var arrival = Assert.Single(world.DrainEvents<EnemyArrived>());
        Assert.Equal(enemy, arrival.Enemy);
        Assert.Equal(3, arrival.Damage);
    }

    [Fact]
    public void ArrivalDamagesBaseAndRemovesEnemy()
    {
        var world = new SimulationWorld();
        var baseEntity = CreateBase(world, 20);
        var enemy = CreateEnemy(world, 3);
        world.Emit(new EnemyArrived(enemy, 3));

        new BaseDamageSystem().Update(world, 0);

        Assert.Equal(17, world.GetComponent<Health>(baseEntity).Current);
        Assert.False(world.IsAlive(enemy));
    }

    [Fact]
    public void MultipleArrivalsCannotReduceHealthBelowZero()
    {
        var world = new SimulationWorld();
        var baseEntity = CreateBase(world, 5);
        var firstEnemy = CreateEnemy(world, 4);
        var secondEnemy = CreateEnemy(world, 4);
        world.Emit(new EnemyArrived(firstEnemy, 4));
        world.Emit(new EnemyArrived(secondEnemy, 4));

        new BaseDamageSystem().Update(world, 0);

        Assert.Equal(0, world.GetComponent<Health>(baseEntity).Current);
        Assert.False(world.IsAlive(firstEnemy));
        Assert.False(world.IsAlive(secondEnemy));
    }

    private static Entity CreateBase(SimulationWorld world, int health)
    {
        var baseEntity = world.CreateEntity();
        world.SetComponent(baseEntity, new Base());
        world.SetComponent(baseEntity, new Health(health, health));
        return baseEntity;
    }

    private static Entity CreateEnemy(SimulationWorld world, int damage)
    {
        var enemy = world.CreateEntity();
        world.SetComponent(enemy, new Enemy(damage));
        return enemy;
    }
}
