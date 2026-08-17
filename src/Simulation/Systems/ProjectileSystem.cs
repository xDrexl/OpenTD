using System;
using System.Linq;
using System.Numerics;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Events;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class ProjectileSystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        foreach (var entity in world.Query<Projectile, Position>().ToArray())
        {
            var projectile = world.GetComponent<Projectile>(entity);
            if (!world.IsAlive(projectile.Target) ||
                !world.TryGetComponent<Position>(projectile.Target, out var targetPosition))
            {
                world.DestroyEntity(entity);
                continue;
            }

            var position = world.GetComponent<Position>(entity).Value;
            var distance = Vector2.Distance(position, targetPosition.Value);
            var travelDistance = Math.Max(0, projectile.Speed) * deltaSeconds;

            if (distance <= travelDistance)
            {
                var damage = world.GetComponent<Damage>(entity);
                world.Emit(new DamageRequested(projectile.Target, damage.Amount));
                world.DestroyEntity(entity);
                continue;
            }

            if (distance > 0)
            {
                var direction = Vector2.Normalize(targetPosition.Value - position);
                world.SetComponent(entity, new Position(position + direction * travelDistance));
            }
        }
    }
}
