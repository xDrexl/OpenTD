using System;
using System.Linq;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Events;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class EconomySystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        var currencyEntities = world.Query<Currency>().Take(2).ToArray();
        if (currencyEntities.Length > 1)
        {
            throw new InvalidOperationException("Only one currency entity is supported.");
        }

        foreach (var death in world.DrainEvents<EnemyDied>())
        {
            if (currencyEntities.Length == 0)
            {
                continue;
            }

            var currencyEntity = currencyEntities[0];
            var currency = world.GetComponent<Currency>(currencyEntity);
            world.SetComponent(
                currencyEntity,
                currency with { Amount = currency.Amount + Math.Max(0, death.Reward) });
        }
    }
}
