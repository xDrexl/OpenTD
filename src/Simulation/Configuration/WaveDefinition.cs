namespace OpenTD.Simulation.Configuration;

public sealed record WaveDefinition(
    int EnemyCount,
    float SpawnIntervalSeconds,
    EnemyConfiguration Enemy);
