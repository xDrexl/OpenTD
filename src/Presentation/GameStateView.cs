using System;
using Godot;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Presentation;

public sealed partial class GameStateView : Control
{
    private SimulationWorld? _world;
    private Entity _gameStateEntity;
    private Action? _nextStage;
    private Action? _returnToMenu;

    public void Initialize(
        SimulationWorld world,
        Entity gameStateEntity,
        Action nextStage,
        Action returnToMenu)
    {
        _world = world;
        _gameStateEntity = gameStateEntity;
        _nextStage = nextStage;
        _returnToMenu = returnToMenu;
        GetNode<Button>("Panel/Action").Pressed += PerformAction;
        UpdateOverlay();
    }

    public override void _Process(double delta) => UpdateOverlay();

    private void UpdateOverlay()
    {
        if (_world is null ||
            !_world.TryGetComponent<GameStatus>(_gameStateEntity, out var status))
        {
            Visible = false;
            return;
        }

        Visible = status.Phase is GamePhase.Victory or GamePhase.Defeat;
        if (Visible)
        {
            var victory = status.Phase == GamePhase.Victory;
            GetNode<Label>("Panel/Result").Text = victory
                ? "Stage Complete!"
                : "Run Over";
            GetNode<Button>("Panel/Action").Text = victory
                ? "Next Stage"
                : "Main Menu";
        }
    }

    private void PerformAction()
    {
        if (_world is null ||
            !_world.TryGetComponent<GameStatus>(_gameStateEntity, out var status))
        {
            return;
        }

        if (status.Phase == GamePhase.Victory)
        {
            _nextStage?.Invoke();
        }
        else if (status.Phase == GamePhase.Defeat)
        {
            _returnToMenu?.Invoke();
        }
    }
}
