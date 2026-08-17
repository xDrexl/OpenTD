using OpenTD.Simulation.Components;
using OpenTD.Simulation.Events;
using OpenTD.Simulation.Systems;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class EconomySystemTests
{
    [Fact]
    public void EnemyDeathAddsRewardToCurrency()
    {
        var world = CreateWorld(4);
        var enemy = world.CreateEntity();
        world.Emit(new EnemyDied(enemy, 3));

        new EconomySystem().Update(world, 0);

        var currency = Assert.Single(world.Query<Currency>());
        Assert.Equal(7, world.GetComponent<Currency>(currency).Amount);
    }

    [Fact]
    public void MultipleDeathsAccumulateRewards()
    {
        var world = CreateWorld(0);
        world.Emit(new EnemyDied(new Entity(10), 2));
        world.Emit(new EnemyDied(new Entity(11), 3));

        new EconomySystem().Update(world, 0);

        var currency = Assert.Single(world.Query<Currency>());
        Assert.Equal(5, world.GetComponent<Currency>(currency).Amount);
    }

    [Fact]
    public void NegativeRewardDoesNotRemoveCurrency()
    {
        var world = CreateWorld(4);
        world.Emit(new EnemyDied(new Entity(10), -3));

        new EconomySystem().Update(world, 0);

        var currency = Assert.Single(world.Query<Currency>());
        Assert.Equal(4, world.GetComponent<Currency>(currency).Amount);
    }

    private static SimulationWorld CreateWorld(int amount)
    {
        var world = new SimulationWorld();
        var currency = world.CreateEntity();
        world.SetComponent(currency, new Currency(amount));
        return world;
    }
}
