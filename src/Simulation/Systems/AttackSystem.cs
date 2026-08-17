using System;
using System.Linq;
using OpenTD.Simulation.Components;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class AttackSystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        foreach (var tower in world.Query<Target, AttackCooldown>().ToArray())
        {
            var cooldown = world.GetComponent<AttackCooldown>(tower);
            var remainingSeconds = Math.Max(0, cooldown.RemainingSeconds - deltaSeconds);
            var target = world.GetComponent<Target>(tower).Entity;

            if (remainingSeconds > 0 ||
                !world.IsAlive(target) ||
                !world.TryGetComponent<Health>(target, out _))
            {
                world.SetComponent(tower, cooldown with { RemainingSeconds = remainingSeconds });
                continue;
            }

            var attackStats = world.GetComponent<AttackStats>(tower);
            var projectile = world.CreateEntity();
            world.SetComponent(projectile, world.GetComponent<Position>(tower));
            world.SetComponent(
                projectile,
                new Projectile(target, attackStats.ProjectileSpeed));
            world.SetComponent(projectile, new Damage(attackStats.Damage));
            world.SetComponent(
                tower,
                cooldown with { RemainingSeconds = cooldown.IntervalSeconds });
        }
    }
}
