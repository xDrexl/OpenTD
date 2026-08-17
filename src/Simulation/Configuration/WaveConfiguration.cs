using System;
using System.Collections.Generic;
using System.Numerics;
using OpenTD.Simulation.Components;

namespace OpenTD.Simulation.Configuration;

public sealed record WaveConfiguration(
    IReadOnlyList<WaveDefinition> Waves,
    float InterWaveDelaySeconds,
    IReadOnlyList<Vector2> Path)
{
    public static WaveConfiguration CreateDefault(MapConfiguration map) =>
        CreateForStage(map, 1);

    public static WaveConfiguration CreateForStage(MapConfiguration map, int stageNumber)
    {
        if (stageNumber < 1)
        {
            throw new ArgumentOutOfRangeException(nameof(stageNumber));
        }

        var establishedWaves = new WaveDefinition[]
        {
            new(
                new EnemyConfiguration[]
                {
                    BasicEnemy(),
                    BasicEnemy(),
                    BasicEnemy(),
                },
                1.5f),
            new(
                new EnemyConfiguration[]
                {
                    BasicEnemy(health: 12, baseDamage: 2),
                    FastEnemy(),
                    BasicEnemy(health: 12, baseDamage: 2),
                    FastEnemy(),
                    BasicEnemy(health: 12, baseDamage: 2),
                },
                1.2f),
            new(
                new EnemyConfiguration[]
                {
                    FastEnemy(health: 9, baseDamage: 2),
                    BasicEnemy(speed: 120, health: 14, baseDamage: 2, reward: 4),
                    FastEnemy(health: 9, baseDamage: 2),
                    BasicEnemy(speed: 120, health: 14, baseDamage: 2, reward: 4),
                    FastEnemy(health: 9, baseDamage: 2),
                    BasicEnemy(speed: 120, health: 14, baseDamage: 2, reward: 4),
                    FastEnemy(health: 9, baseDamage: 2),
                },
                1),
        };
        var waves = new WaveDefinition[stageNumber + 2];
        for (var index = 0; index < waves.Length; index++)
        {
            waves[index] = establishedWaves[index % establishedWaves.Length];
        }

        return new WaveConfiguration(
            waves,
            InterWaveDelaySeconds: 3,
            Path: map.Path);
    }

    private static EnemyConfiguration BasicEnemy(
        float speed = 100,
        int health = 10,
        int baseDamage = 1,
        int reward = 3) =>
        new(speed, health, baseDamage, reward, EnemyArchetypeId.Basic);

    private static EnemyConfiguration FastEnemy(
        int health = 7,
        int baseDamage = 1) =>
        new(160, health, baseDamage, 4, EnemyArchetypeId.Fast);
}
