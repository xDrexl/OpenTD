using System;
using Godot;
using OpenTD.Infrastructure.Persistence;

namespace OpenTD.Presentation;

public sealed partial class MainMenu : Control
{
    private const string GameScenePath = "res://scenes/Main.tscn";
    private RunCheckpointStore _checkpointStore = null!;

    public override void _Ready()
    {
        _checkpointStore = GodotRunCheckpointStore.Create();
        GetNode<Button>("Menu/NewGame").Pressed += NewGame;
        GetNode<Button>("Menu/Continue").Pressed += ContinueGame;
        GetNode<Button>("Menu/Quit").Pressed += Quit;
        GetNode<ConfirmationDialog>("ReplaceRunConfirmation").Confirmed += StartNewGame;

        GetNode<Button>("Menu/Continue").Disabled = !_checkpointStore.TryLoad(out _);
    }

    private void NewGame()
    {
        if (_checkpointStore.TryLoad(out _))
        {
            GetNode<ConfirmationDialog>("ReplaceRunConfirmation").PopupCentered();
            return;
        }

        StartNewGame();
    }

    private void StartNewGame()
    {
        _checkpointStore.Save(RunCheckpoint.Create(
            stageNumber: 1,
            runSeed: Random.Shared.NextInt64()));
        GetTree().ChangeSceneToFile(GameScenePath);
    }

    private void ContinueGame()
    {
        if (_checkpointStore.TryLoad(out _))
        {
            GetTree().ChangeSceneToFile(GameScenePath);
        }
    }

    private void Quit() => GetTree().Quit();
}
