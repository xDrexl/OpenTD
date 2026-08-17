using System;
using System.IO;
using OpenTD.Infrastructure.Persistence;
using Xunit;

namespace OpenTD.Tests.Infrastructure;

public sealed class RunCheckpointStoreTests : IDisposable
{
    private readonly string _directory = Path.Combine(
        Path.GetTempPath(),
        $"opentd-checkpoint-tests-{Guid.NewGuid():N}");

    private string CheckpointPath => Path.Combine(_directory, RunCheckpointStore.FileName);

    [Fact]
    public void SavedCheckpointRoundTrips()
    {
        var store = new RunCheckpointStore(CheckpointPath);
        var expected = RunCheckpoint.Create(stageNumber: 4, runSeed: 987654321);

        store.Save(expected);

        Assert.True(store.TryLoad(out var actual));
        Assert.Equal(expected, actual);
    }

    [Fact]
    public void SavingAgainAtomicallyReplacesCheckpoint()
    {
        var store = new RunCheckpointStore(CheckpointPath);
        store.Save(RunCheckpoint.Create(stageNumber: 2, runSeed: 10));

        var replacement = RunCheckpoint.Create(stageNumber: 3, runSeed: 10);
        store.Save(replacement);

        Assert.True(store.TryLoad(out var actual));
        Assert.Equal(replacement, actual);
        Assert.Empty(Directory.GetFiles(_directory, "*.tmp"));
    }

    [Fact]
    public void MissingCheckpointDoesNotLoad()
    {
        var store = new RunCheckpointStore(CheckpointPath);

        Assert.False(store.TryLoad(out _));
    }

    [Fact]
    public void MalformedCheckpointDoesNotLoad()
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(CheckpointPath, "not json");
        var store = new RunCheckpointStore(CheckpointPath);

        Assert.False(store.TryLoad(out _));
    }

    [Fact]
    public void UnsupportedVersionDoesNotLoad()
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(
            CheckpointPath,
            "{\"version\":99,\"stageNumber\":1,\"runSeed\":42}");
        var store = new RunCheckpointStore(CheckpointPath);

        Assert.False(store.TryLoad(out _));
    }

    [Fact]
    public void InvalidStageDoesNotLoad()
    {
        Directory.CreateDirectory(_directory);
        File.WriteAllText(
            CheckpointPath,
            "{\"version\":1,\"stageNumber\":0,\"runSeed\":42}");
        var store = new RunCheckpointStore(CheckpointPath);

        Assert.False(store.TryLoad(out _));
    }

    [Fact]
    public void DeleteRemovesCheckpoint()
    {
        var store = new RunCheckpointStore(CheckpointPath);
        store.Save(RunCheckpoint.Create(stageNumber: 1, runSeed: 42));

        store.Delete();

        Assert.False(store.TryLoad(out _));
        Assert.False(File.Exists(CheckpointPath));
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
