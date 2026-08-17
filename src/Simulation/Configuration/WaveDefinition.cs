using System.Collections.Generic;
using System.Linq;

namespace OpenTD.Simulation.Configuration;

public sealed record WaveDefinition(
    IReadOnlyList<EnemyConfiguration> Enemies,
    float SpawnIntervalSeconds)
{
    public WaveDefinition(
        int enemyCount,
        float spawnIntervalSeconds,
        EnemyConfiguration enemy)
        : this(
            Enumerable.Repeat(enemy, enemyCount).ToArray(),
            spawnIntervalSeconds)
    {
    }

    public int EnemyCount => Enemies.Count;
}
