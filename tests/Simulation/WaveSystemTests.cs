using System.Linq;
using System.Numerics;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Configuration;
using OpenTD.Simulation.Systems;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class WaveSystemTests
{
    private static readonly EnemyConfiguration Enemy = new(10, 7, 2, 3);

    [Fact]
    public void FirstEnemySpawnsImmediatelyWithConfiguredComponents()
    {
        var configuration = CreateConfiguration(new WaveDefinition(2, 1, Enemy));
        var (world, system) = CreateWorld(configuration);

        system.Update(world, 0);

        var enemy = Assert.Single(world.Query<Enemy>());
        Assert.Equal(2, world.GetComponent<Enemy>(enemy).BaseDamage);
        Assert.Equal(7, world.GetComponent<Health>(enemy).Current);
        Assert.Equal(3, world.GetComponent<Reward>(enemy).Amount);
        Assert.Equal(10, world.GetComponent<Movement>(enemy).Speed);
        Assert.Equal(Vector2.Zero, world.GetComponent<Position>(enemy).Value);
    }

    [Fact]
    public void EnemiesSpawnAtConfiguredIntervals()
    {
        var configuration = CreateConfiguration(new WaveDefinition(3, 1, Enemy));
        var (world, system) = CreateWorld(configuration);

        system.Update(world, 0);
        system.Update(world, 0.5f);
        Assert.Single(world.Query<Enemy>());
        system.Update(world, 0.5f);

        Assert.Equal(2, world.Query<Enemy>().Count());
    }

    [Fact]
    public void NextWaveWaitsForEnemiesAndInterWaveDelay()
    {
        var configuration = CreateConfiguration(
            new WaveDefinition(1, 1, Enemy),
            new WaveDefinition(1, 1, Enemy));
        var (world, system) = CreateWorld(configuration);
        system.Update(world, 0);
        system.Update(world, 2);
        Assert.Equal(1, GetState(world).CurrentWave);
        world.DestroyEntity(Assert.Single(world.Query<Enemy>()));

        system.Update(world, 0);
        Assert.True(GetState(world).IsBetweenWaves);
        system.Update(world, 1);
        Assert.Equal(1, GetState(world).CurrentWave);
        system.Update(world, 1);

        Assert.Equal(2, GetState(world).CurrentWave);
        Assert.False(GetState(world).IsBetweenWaves);
    }

    [Fact]
    public void FinalWaveCompletesAfterEveryEnemyIsRemoved()
    {
        var configuration = CreateConfiguration(new WaveDefinition(1, 1, Enemy));
        var (world, system) = CreateWorld(configuration);
        system.Update(world, 0);
        world.DestroyEntity(Assert.Single(world.Query<Enemy>()));

        system.Update(world, 0);

        var state = GetState(world);
        Assert.True(state.IsComplete);
        Assert.Equal(0, state.RemainingEnemies);
    }

    [Fact]
    public void RemainingEnemiesIncludesSpawnedAndUnspawnedEnemies()
    {
        var configuration = CreateConfiguration(new WaveDefinition(3, 1, Enemy));
        var (world, system) = CreateWorld(configuration);

        system.Update(world, 0);

        Assert.Equal(3, GetState(world).RemainingEnemies);
    }

    [Fact]
    public void MixedWaveSpawnsConfiguredArchetypesInOrder()
    {
        var basic = new EnemyConfiguration(10, 7, 2, 3, EnemyArchetypeId.Basic);
        var fast = new EnemyConfiguration(20, 4, 1, 5, EnemyArchetypeId.Fast);
        var configuration = CreateConfiguration(
            new WaveDefinition(new EnemyConfiguration[] { basic, fast }, 1));
        var (world, system) = CreateWorld(configuration);

        system.Update(world, 0);
        system.Update(world, 1);

        var enemies = world.Query<Enemy>().ToArray();
        Assert.Equal(2, enemies.Length);
        Assert.Equal(
            EnemyArchetypeId.Basic,
            world.GetComponent<EnemyArchetype>(enemies[0]).Id);
        Assert.Equal(
            EnemyArchetypeId.Fast,
            world.GetComponent<EnemyArchetype>(enemies[1]).Id);
        Assert.Equal(20, world.GetComponent<Movement>(enemies[1]).Speed);
        Assert.Equal(4, world.GetComponent<Health>(enemies[1]).Current);
        Assert.Equal(5, world.GetComponent<Reward>(enemies[1]).Amount);
    }

    private static (SimulationWorld World, WaveSystem System) CreateWorld(
        WaveConfiguration configuration)
    {
        var world = new SimulationWorld();
        var state = world.CreateEntity();
        world.SetComponent(
            state,
            WaveState.Create(
                configuration.Waves.Count,
                configuration.Waves[0].EnemyCount));
        return (world, new WaveSystem(configuration));
    }

    private static WaveConfiguration CreateConfiguration(params WaveDefinition[] waves) =>
        new(waves, 2, new Vector2[] { Vector2.Zero, new(10, 0) });

    private static WaveState GetState(SimulationWorld world)
    {
        var state = Assert.Single(world.Query<WaveState>());
        return world.GetComponent<WaveState>(state);
    }
}
