using System;
using OpenTD.Simulation.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class WorldTests
{
    [Fact]
    public void EntityLifecycleRemovesAttachedComponents()
    {
        var world = new World();
        var entity = world.CreateEntity();
        world.SetComponent(entity, new Position(2, 3));

        Assert.True(world.DestroyEntity(entity));
        Assert.False(world.IsAlive(entity));
        Assert.False(world.TryGetComponent<Position>(entity, out _));
        Assert.False(world.DestroyEntity(entity));
    }

    [Fact]
    public void ComponentsCanBeAttachedReadUpdatedAndRemoved()
    {
        var world = new World();
        var entity = world.CreateEntity();

        world.SetComponent(entity, new Position(2, 3));
        world.SetComponent(entity, new Position(5, 8));

        Assert.Equal(new Position(5, 8), world.GetComponent<Position>(entity));
        Assert.True(world.RemoveComponent<Position>(entity));
        Assert.False(world.TryGetComponent<Position>(entity, out _));
    }

    [Fact]
    public void QueryReturnsOnlyEntitiesWithEveryRequestedComponent()
    {
        var world = new World();
        var movingEntity = world.CreateEntity();
        var stationaryEntity = world.CreateEntity();
        world.SetComponent(movingEntity, new Position(0, 0));
        world.SetComponent(movingEntity, new Velocity(1, 0));
        world.SetComponent(stationaryEntity, new Position(5, 5));

        Assert.Equal([movingEntity], world.Query<Position, Velocity>());
        Assert.Equal([movingEntity, stationaryEntity], world.Query<Position>());
    }

    [Fact]
    public void ComponentsCannotBeAttachedToDeadEntities()
    {
        var world = new World();
        var entity = world.CreateEntity();
        world.DestroyEntity(entity);

        Assert.Throws<InvalidOperationException>(() =>
            world.SetComponent(entity, new Position(0, 0)));
    }

    private readonly record struct Position(float X, float Y);
    private readonly record struct Velocity(float X, float Y);
}
