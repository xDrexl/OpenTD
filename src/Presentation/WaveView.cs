using Godot;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Presentation;

public sealed partial class WaveView : Label
{
    private SimulationWorld? _world;
    private Entity _waveEntity;

    public void Initialize(SimulationWorld world, Entity waveEntity)
    {
        _world = world;
        _waveEntity = waveEntity;
        UpdateLabel();
    }

    public override void _Process(double delta) => UpdateLabel();

    private void UpdateLabel()
    {
        if (_world is null ||
            !_world.TryGetComponent<WaveState>(_waveEntity, out var state))
        {
            return;
        }

        Text = state.IsComplete
            ? "Waves complete"
            : $"Wave {state.CurrentWave}/{state.TotalWaves}  Enemies: {state.RemainingEnemies}";
    }
}
