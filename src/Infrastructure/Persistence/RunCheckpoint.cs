namespace OpenTD.Infrastructure.Persistence;

public readonly record struct RunCheckpoint(int Version, int StageNumber, long RunSeed)
{
    public const int CurrentVersion = 1;

    public static RunCheckpoint Create(int stageNumber, long runSeed) =>
        new(CurrentVersion, stageNumber, runSeed);

    public bool IsSupported => Version == CurrentVersion && StageNumber > 0;
}
