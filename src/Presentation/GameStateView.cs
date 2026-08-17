using Godot;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Presentation;

public sealed partial class GameStateView : Control
{
    private SimulationWorld? _world;
    private Entity _gameStateEntity;

    public void Initialize(SimulationWorld world, Entity gameStateEntity)
    {
        _world = world;
        _gameStateEntity = gameStateEntity;
        GetNode<Button>("Panel/Restart").Pressed += Restart;
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
            GetNode<Label>("Panel/Result").Text = status.Phase == GamePhase.Victory
                ? "Victory!"
                : "Defeat";
        }
    }

    private void Restart() => GetTree().ReloadCurrentScene();
}
