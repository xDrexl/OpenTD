using Godot;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Presentation;

public sealed partial class TowerView : Sprite2D
{
    private SimulationWorld? _world;
    private Entity _entity;

    public void Initialize(SimulationWorld world, Entity entity)
    {
        _world = world;
        _entity = entity;
        var position = world.GetComponent<Position>(entity).Value;
        Position = new Vector2(position.X, position.Y);
    }

    public override void _Process(double delta) => QueueRedraw();

    public override void _Draw()
    {
        if (_world is null ||
            !_world.TryGetComponent<Target>(_entity, out var target) ||
            !_world.TryGetComponent<Position>(target.Entity, out var targetPosition))
        {
            return;
        }

        var offset = targetPosition.Value - _world.GetComponent<Position>(_entity).Value;
        DrawLine(
            Vector2.Zero,
            new Vector2(offset.X, offset.Y),
            new Color(1, 0.85f, 0.25f, 0.8f),
            2);
    }
}
