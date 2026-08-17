using System;
using System.Linq;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Events;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class BaseDamageSystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        var baseEntities = world.Query<Base, Health>().Take(2).ToArray();
        if (baseEntities.Length > 1)
        {
            throw new InvalidOperationException("Only one base entity is supported.");
        }

        var baseEntity = baseEntities.Length == 1 ? baseEntities[0] : (Entity?)null;

        foreach (var arrival in world.DrainEvents<EnemyArrived>())
        {
            if (baseEntity is { } entity && world.IsAlive(entity))
            {
                var health = world.GetComponent<Health>(entity);
                var currentHealth = Math.Max(0, health.Current - Math.Max(0, arrival.Damage));
                world.SetComponent(entity, health with { Current = currentHealth });
            }

            world.DestroyEntity(arrival.Enemy);
        }
    }
}
