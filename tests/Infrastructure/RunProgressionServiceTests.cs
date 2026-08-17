using System;
using System.IO;
using OpenTD.Infrastructure.Persistence;
using Xunit;

namespace OpenTD.Tests.Infrastructure;

public sealed class RunProgressionServiceTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        $"opentd-progression-tests-{Guid.NewGuid():N}");

    private string CheckpointPath => Path.Combine(_directory, RunCheckpointStore.FileName);

    [Fact]
    public void CompletingStageAdvancesCheckpointAndPreservesRunSeed()
    {
        var store = new RunCheckpointStore(CheckpointPath);
        var progression = new RunProgressionService(store);
        var current = RunCheckpoint.Create(stageNumber: 5, runSeed: 12345);
        store.Save(current);

        var next = progression.CompleteStage(current);

        Assert.Equal(6, next.StageNumber);
        Assert.Equal(current.RunSeed, next.RunSeed);
        Assert.True(store.TryLoad(out var persisted));
        Assert.Equal(next, persisted);
    }

    [Fact]
    public void EndingRunDeletesCheckpoint()
    {
        var store = new RunCheckpointStore(CheckpointPath);
        store.Save(RunCheckpoint.Create(stageNumber: 5, runSeed: 12345));
        var progression = new RunProgressionService(store);

        progression.EndRun();

        Assert.False(store.TryLoad(out _));
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
