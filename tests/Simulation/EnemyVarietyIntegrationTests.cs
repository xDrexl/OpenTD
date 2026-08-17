using System.Numerics;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Events;
using OpenTD.Simulation.Systems;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class EnemyVarietyIntegrationTests
{
    [Fact]
    public void FastEnemyUsesExistingGameplaySystemsUnchanged()
    {
        var world = new SimulationWorld();
        var currency = world.CreateEntity();
        world.SetComponent(currency, new Currency(0));

        var enemy = world.CreateEntity();
        world.SetComponent(enemy, new Enemy(1));
        world.SetComponent(enemy, new EnemyArchetype(EnemyArchetypeId.Fast));
        world.SetComponent(enemy, new Position(Vector2.Zero));
        world.SetComponent(enemy, new Movement(20));
        world.SetComponent(
            enemy,
            new PathProgress([Vector2.Zero, new Vector2(100, 0)], 1));
        world.SetComponent(enemy, new Health(4, 4));
        world.SetComponent(enemy, new Reward(5));

        var tower = world.CreateEntity();
        world.SetComponent(tower, new Tower());
        world.SetComponent(tower, new Position(new Vector2(10, 0)));
        world.SetComponent(tower, new AttackRange(50));

        new MovementSystem().Update(world, 0.5f);
        new TargetingSystem().Update(world, 0);
        Assert.Equal(new Vector2(10, 0), world.GetComponent<Position>(enemy).Value);
        Assert.Equal(enemy, world.GetComponent<Target>(tower).Entity);

        world.Emit(new DamageRequested(enemy, 4));
        new DamageSystem().Update(world, 0);
        new DeathSystem().Update(world, 0);
        new EconomySystem().Update(world, 0);

        Assert.False(world.IsAlive(enemy));
        Assert.Equal(5, world.GetComponent<Currency>(currency).Amount);
    }
}
