using System.Numerics;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Systems;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class MovementSystemTests
{
    [Fact]
    public void EnemyMovesTowardNextWaypoint()
    {
        var world = CreateMovingEnemy([Vector2.Zero, new Vector2(10, 0)]);

        new MovementSystem().Update(world, 0.5f);

        var entity = Assert.Single(world.Query<Enemy>());
        Assert.Equal(new Vector2(5, 0), world.GetComponent<Position>(entity).Value);
        Assert.Equal(1, world.GetComponent<PathProgress>(entity).NextWaypointIndex);
    }

    [Fact]
    public void EnemyUsesRemainingMovementAcrossWaypoints()
    {
        var world = CreateMovingEnemy(
            [Vector2.Zero, new Vector2(3, 0), new Vector2(3, 4)]);

        new MovementSystem().Update(world, 0.5f);

        var entity = Assert.Single(world.Query<Enemy>());
        Assert.Equal(new Vector2(3, 2), world.GetComponent<Position>(entity).Value);
        Assert.Equal(2, world.GetComponent<PathProgress>(entity).NextWaypointIndex);
    }

    [Fact]
    public void PathCompletionIsMarkedAtFinalWaypoint()
    {
        var world = CreateMovingEnemy([Vector2.Zero, new Vector2(5, 0)]);
        var movement = new MovementSystem();
        var completion = new PathCompletionSystem();

        movement.Update(world, 1);
        completion.Update(world, 1);

        var entity = Assert.Single(world.Query<Enemy>());
        Assert.Equal(new Vector2(5, 0), world.GetComponent<Position>(entity).Value);
        Assert.True(world.GetComponent<PathProgress>(entity).IsCompleted);
    }

    private static SimulationWorld CreateMovingEnemy(Vector2[] path)
    {
        var world = new SimulationWorld();
        var enemy = world.CreateEntity();
        world.SetComponent(enemy, new Enemy());
        world.SetComponent(enemy, new Position(path[0]));
        world.SetComponent(enemy, new Movement(10));
        world.SetComponent(enemy, new PathProgress(path, 1));
        return world;
    }
}
