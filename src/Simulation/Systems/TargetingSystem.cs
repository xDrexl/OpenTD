using System.Linq;
using System.Numerics;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class TargetingSystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        var enemies = world.Query<Enemy, Position>()
            .Where(entity => IsTargetable(world, entity))
            .ToArray();

        foreach (var tower in world.Query<AttackRange, Position>().ToArray())
        {
            var towerPosition = world.GetComponent<Position>(tower).Value;
            var range = world.GetComponent<AttackRange>(tower).Radius;
            var rangeSquared = range * range;
            Entity? nearestEnemy = null;
            var nearestDistanceSquared = float.MaxValue;

            foreach (var enemy in enemies)
            {
                var enemyPosition = world.GetComponent<Position>(enemy).Value;
                var distanceSquared = Vector2.DistanceSquared(towerPosition, enemyPosition);
                if (distanceSquared > rangeSquared ||
                    distanceSquared > nearestDistanceSquared)
                {
                    continue;
                }

                if (distanceSquared < nearestDistanceSquared ||
                    nearestEnemy is null ||
                    enemy.Id < nearestEnemy.Value.Id)
                {
                    nearestEnemy = enemy;
                    nearestDistanceSquared = distanceSquared;
                }
            }

            if (nearestEnemy is { } target)
            {
                world.SetComponent(tower, new Target(target));
            }
            else
            {
                world.RemoveComponent<Target>(tower);
            }
        }
    }

    private static bool IsTargetable(SimulationWorld world, Entity enemy)
    {
        return !world.TryGetComponent<PathProgress>(enemy, out var progress) ||
               !progress.IsCompleted;
    }
}
