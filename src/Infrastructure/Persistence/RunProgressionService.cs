namespace OpenTD.Infrastructure.Persistence;

public sealed class RunProgressionService(RunCheckpointStore checkpointStore)
{
    public RunCheckpoint CompleteStage(RunCheckpoint currentCheckpoint)
    {
        var nextCheckpoint = RunCheckpoint.Create(
            currentCheckpoint.StageNumber + 1,
            currentCheckpoint.RunSeed);
        checkpointStore.Save(nextCheckpoint);
        return nextCheckpoint;
    }

    public void EndRun() => checkpointStore.Delete();
}
