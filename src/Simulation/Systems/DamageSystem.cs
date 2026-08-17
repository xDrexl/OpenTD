using System;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Events;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class DamageSystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        foreach (var damage in world.DrainEvents<DamageRequested>())
        {
            if (!world.IsAlive(damage.Target) ||
                !world.TryGetComponent<Health>(damage.Target, out var health))
            {
                continue;
            }

            var currentHealth = Math.Max(0, health.Current - Math.Max(0, damage.Amount));
            world.SetComponent(damage.Target, health with { Current = currentHealth });
        }
    }
}
