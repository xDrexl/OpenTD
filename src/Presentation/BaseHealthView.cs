using Godot;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Presentation;

public sealed partial class BaseHealthView : Label
{
    private SimulationWorld? _world;
    private Entity _baseEntity;

    public void Initialize(SimulationWorld world, Entity baseEntity)
    {
        _world = world;
        _baseEntity = baseEntity;
        UpdateLabel();
    }

    public override void _Process(double delta)
    {
        UpdateLabel();
    }

    private void UpdateLabel()
    {
        if (_world is not null &&
            _world.TryGetComponent<Health>(_baseEntity, out var health))
        {
            Text = $"Base: {health.Current}/{health.Maximum}";
        }
    }
}
