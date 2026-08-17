using System;
using System.Linq;
using System.Numerics;
using OpenTD.Simulation.Components;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class MovementSystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        foreach (var entity in world.Query<Position, Movement>().ToArray())
        {
            if (!world.TryGetComponent<PathProgress>(entity, out var progress) ||
                progress.IsCompleted)
            {
                continue;
            }

            var movement = world.GetComponent<Movement>(entity);
            var position = world.GetComponent<Position>(entity).Value;
            var speedMultiplier = world.TryGetComponent<SlowEffect>(entity, out var slow)
                ? slow.SpeedMultiplier
                : 1;
            var remainingDistance = Math.Max(0, movement.Speed) *
                                    Math.Clamp(speedMultiplier, 0, 1) *
                                    deltaSeconds;
            var nextWaypointIndex = progress.NextWaypointIndex;

            while (remainingDistance > 0 && nextWaypointIndex < progress.Waypoints.Count)
            {
                var waypoint = progress.Waypoints[nextWaypointIndex];
                var distanceToWaypoint = Vector2.Distance(position, waypoint);

                if (distanceToWaypoint > remainingDistance)
                {
                    position = Vector2.Lerp(position, waypoint, remainingDistance / distanceToWaypoint);
                    remainingDistance = 0;
                    continue;
                }

                position = waypoint;
                remainingDistance -= distanceToWaypoint;
                nextWaypointIndex++;
            }

            world.SetComponent(entity, new Position(position));
            world.SetComponent(entity, progress with { NextWaypointIndex = nextWaypointIndex });
        }
    }
}
