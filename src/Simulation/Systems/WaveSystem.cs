using System;
using System.Linq;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Configuration;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class WaveSystem(WaveConfiguration configuration) : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        var stateEntities = world.Query<WaveState>().Take(2).ToArray();
        if (stateEntities.Length != 1)
        {
            throw new InvalidOperationException("Exactly one wave state entity is required.");
        }

        var stateEntity = stateEntities[0];
        var state = world.GetComponent<WaveState>(stateEntity);
        if (state.IsComplete)
        {
            return;
        }

        if (state.CurrentWave < 1 || state.CurrentWave > configuration.Waves.Count)
        {
            throw new InvalidOperationException("Wave state references an invalid wave.");
        }

        state = state.IsBetweenWaves
            ? UpdateInterWaveDelay(state, deltaSeconds)
            : UpdateCurrentWave(world, state, deltaSeconds);

        var activeEnemies = world.Query<Enemy>().Count();
        var unspawnedEnemies = state.IsComplete
            ? 0
            : Math.Max(0, state.TotalEnemiesInWave - state.SpawnedEnemies);
        state = state with { RemainingEnemies = activeEnemies + unspawnedEnemies };
        world.SetComponent(stateEntity, state);
    }

    private WaveState UpdateCurrentWave(
        SimulationWorld world,
        WaveState state,
        float deltaSeconds)
    {
        var definition = configuration.Waves[state.CurrentWave - 1];
        var secondsRemaining = state.SecondsRemaining - deltaSeconds;
        var spawnedEnemies = state.SpawnedEnemies;

        while (spawnedEnemies < definition.EnemyCount && secondsRemaining <= 0)
        {
            SpawnEnemy(world, definition.Enemies[spawnedEnemies]);
            spawnedEnemies++;
            secondsRemaining += Math.Max(0, definition.SpawnIntervalSeconds);
        }

        state = state with
        {
            SpawnedEnemies = spawnedEnemies,
            SecondsRemaining = Math.Max(0, secondsRemaining),
        };

        if (spawnedEnemies < definition.EnemyCount || world.Query<Enemy>().Any())
        {
            return state;
        }

        if (state.CurrentWave == configuration.Waves.Count)
        {
            return state with { IsComplete = true, SecondsRemaining = 0 };
        }

        return state with
        {
            IsBetweenWaves = true,
            SecondsRemaining = Math.Max(0, configuration.InterWaveDelaySeconds),
        };
    }

    private WaveState UpdateInterWaveDelay(WaveState state, float deltaSeconds)
    {
        var secondsRemaining = Math.Max(0, state.SecondsRemaining - deltaSeconds);
        if (secondsRemaining > 0)
        {
            return state with { SecondsRemaining = secondsRemaining };
        }

        var nextWave = state.CurrentWave + 1;
        var nextDefinition = configuration.Waves[nextWave - 1];
        return state with
        {
            CurrentWave = nextWave,
            SpawnedEnemies = 0,
            TotalEnemiesInWave = nextDefinition.EnemyCount,
            RemainingEnemies = nextDefinition.EnemyCount,
            SecondsRemaining = 0,
            IsBetweenWaves = false,
        };
    }

    private void SpawnEnemy(SimulationWorld world, EnemyConfiguration enemyConfiguration)
    {
        if (configuration.Path.Count == 0)
        {
            throw new InvalidOperationException("Enemy path must contain at least one waypoint.");
        }

        var enemy = world.CreateEntity();
        world.SetComponent(enemy, new Enemy(enemyConfiguration.BaseDamage));
        world.SetComponent(enemy, new EnemyArchetype(enemyConfiguration.Archetype));
        world.SetComponent(enemy, new Health(enemyConfiguration.Health, enemyConfiguration.Health));
        world.SetComponent(enemy, new Reward(enemyConfiguration.Reward));
        world.SetComponent(enemy, new Position(configuration.Path[0]));
        world.SetComponent(enemy, new Movement(enemyConfiguration.Speed));
        world.SetComponent(
            enemy,
            new PathProgress(configuration.Path, Math.Min(1, configuration.Path.Count)));
    }
}
