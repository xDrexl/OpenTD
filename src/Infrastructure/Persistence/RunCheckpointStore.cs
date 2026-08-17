using System;
using System.IO;
using System.Text.Json;

namespace OpenTD.Infrastructure.Persistence;

public sealed class RunCheckpointStore
{
    public const string FileName = "run-checkpoint.json";

    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
    };

    private readonly string _path;

    public RunCheckpointStore(string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(path);
        _path = path;
    }

    public bool TryLoad(out RunCheckpoint checkpoint)
    {
        checkpoint = default;
        try
        {
            if (!File.Exists(_path))
            {
                return false;
            }

            var json = File.ReadAllText(_path);
            var loaded = JsonSerializer.Deserialize<RunCheckpoint>(json, SerializerOptions);
            if (!loaded.IsSupported)
            {
                return false;
            }

            checkpoint = loaded;
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
        catch (IOException)
        {
            return false;
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
    }

    public void Save(RunCheckpoint checkpoint)
    {
        if (!checkpoint.IsSupported)
        {
            throw new ArgumentException("Checkpoint is not valid or supported.", nameof(checkpoint));
        }

        var directory = Path.GetDirectoryName(_path);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var temporaryPath = $"{_path}.{Guid.NewGuid():N}.tmp";
        try
        {
            var json = JsonSerializer.Serialize(checkpoint, SerializerOptions);
            using (var stream = new FileStream(
                       temporaryPath,
                       FileMode.CreateNew,
                       FileAccess.Write,
                       FileShare.None))
            using (var writer = new StreamWriter(stream))
            {
                writer.Write(json);
                writer.Flush();
                stream.Flush(flushToDisk: true);
            }

            File.Move(temporaryPath, _path, overwrite: true);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }

    public void Delete()
    {
        if (File.Exists(_path))
        {
            File.Delete(_path);
        }
    }
}
