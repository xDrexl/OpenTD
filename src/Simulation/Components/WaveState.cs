namespace OpenTD.Simulation.Components;

public readonly record struct WaveState(
    int CurrentWave,
    int TotalWaves,
    int SpawnedEnemies,
    int TotalEnemiesInWave,
    int RemainingEnemies,
    float SecondsRemaining,
    bool IsBetweenWaves,
    bool IsComplete)
{
    public static WaveState Create(int totalWaves, int firstWaveEnemyCount) => new(
        CurrentWave: 1,
        TotalWaves: totalWaves,
        SpawnedEnemies: 0,
        TotalEnemiesInWave: firstWaveEnemyCount,
        RemainingEnemies: firstWaveEnemyCount,
        SecondsRemaining: 0,
        IsBetweenWaves: false,
        IsComplete: totalWaves == 0);
}
