using System;
using System.Linq;
using System.Numerics;
using OpenTD.Simulation.Commands;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Configuration;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class TowerPlacementSystem(
    MapConfiguration map,
    TowerPlacementConfiguration configuration) : ISystem
{
    public bool CanPlace(SimulationWorld world, Vector2 position)
    {
        var radius = configuration.TowerRadius;
        if (position.X < radius || position.X > map.Width - radius ||
            position.Y < radius || position.Y > map.Height - radius)
        {
            return false;
        }

        if (DistanceToPath(position) < map.PathHalfWidth + radius)
        {
            return false;
        }

        foreach (var towerEntity in world.Query<Tower, Position>())
        {
            var towerPosition = world.GetComponent<Position>(towerEntity).Value;
            if (Vector2.Distance(position, towerPosition) < radius * 2)
            {
                return false;
            }
        }

        var currencyEntities = world.Query<Currency>().Take(2).ToArray();
        return currencyEntities.Length == 1 &&
               world.GetComponent<Currency>(currencyEntities[0]).Amount >= configuration.BuildCost;
    }

    public void Update(SimulationWorld world, float deltaSeconds)
    {
        foreach (var command in world.DrainCommands<PlaceTower>())
        {
            if (!CanPlace(world, command.Position))
            {
                continue;
            }

            var currencyEntity = world.Query<Currency>().Single();
            var currency = world.GetComponent<Currency>(currencyEntity);
            world.SetComponent(
                currencyEntity,
                currency with { Amount = currency.Amount - configuration.BuildCost });

            var tower = world.CreateEntity();
            world.SetComponent(tower, new Tower());
            world.SetComponent(tower, new Position(command.Position));
            world.SetComponent(tower, new BuildCost(configuration.BuildCost));
            world.SetComponent(tower, new AttackRange(configuration.AttackRange));
        }
    }

    private float DistanceToPath(Vector2 point)
    {
        var shortestDistance = float.MaxValue;

        for (var index = 0; index < map.Path.Count - 1; index++)
        {
            shortestDistance = Math.Min(
                shortestDistance,
                DistanceToSegment(point, map.Path[index], map.Path[index + 1]));
        }

        return shortestDistance;
    }

    private static float DistanceToSegment(Vector2 point, Vector2 start, Vector2 end)
    {
        var segment = end - start;
        var lengthSquared = segment.LengthSquared();
        if (lengthSquared == 0)
        {
            return Vector2.Distance(point, start);
        }

        var progress = Math.Clamp(Vector2.Dot(point - start, segment) / lengthSquared, 0, 1);
        return Vector2.Distance(point, start + segment * progress);
    }
}
