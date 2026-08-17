using Godot;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Presentation;

public sealed partial class ProjectileView : Sprite2D
{
    private SimulationWorld? _world;
    private Entity _entity;

    public void Initialize(SimulationWorld world, Entity entity)
    {
        _world = world;
        _entity = entity;
        UpdatePosition();
    }

    public override void _Process(double delta)
    {
        if (_world is null)
        {
            return;
        }

        if (!_world.IsAlive(_entity))
        {
            if (GetParent() is { } parent)
            {
                ImpactEffect.Spawn(parent, Position, new Color(1, 0.82f, 0.25f));
            }

            QueueFree();
            return;
        }

        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (_world is not null && _world.TryGetComponent<Position>(_entity, out var position))
        {
            Position = new Vector2(position.Value.X, position.Value.Y);
        }
    }
}
