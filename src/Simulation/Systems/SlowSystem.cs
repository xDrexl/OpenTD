using System;
using System.Linq;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Events;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class SlowSystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        foreach (var entity in world.Query<SlowEffect>().ToArray())
        {
            var effect = world.GetComponent<SlowEffect>(entity);
            var remainingSeconds = effect.RemainingSeconds - deltaSeconds;
            if (remainingSeconds <= 0)
            {
                world.RemoveComponent<SlowEffect>(entity);
            }
            else
            {
                world.SetComponent(
                    entity,
                    effect with { RemainingSeconds = remainingSeconds });
            }
        }

        foreach (var request in world.DrainEvents<SlowRequested>())
        {
            if (!world.IsAlive(request.Target) || request.DurationSeconds <= 0)
            {
                continue;
            }

            var multiplier = Math.Clamp(request.SpeedMultiplier, 0, 1);
            if (world.TryGetComponent<SlowEffect>(request.Target, out var existing))
            {
                multiplier = Math.Min(multiplier, existing.SpeedMultiplier);
                world.SetComponent(
                    request.Target,
                    new SlowEffect(
                        multiplier,
                        Math.Max(request.DurationSeconds, existing.RemainingSeconds)));
            }
            else
            {
                world.SetComponent(
                    request.Target,
                    new SlowEffect(multiplier, request.DurationSeconds));
            }
        }
    }
}
