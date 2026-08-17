using Godot;

namespace OpenTD.Infrastructure.Persistence;

public static class GodotRunCheckpointStore
{
    public static RunCheckpointStore Create() => new(
        ProjectSettings.GlobalizePath($"user://{RunCheckpointStore.FileName}"));
}
