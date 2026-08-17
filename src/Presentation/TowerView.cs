using Godot;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Presentation;

public sealed partial class TowerView : Sprite2D
{
    public void Initialize(SimulationWorld world, Entity entity)
    {
        var position = world.GetComponent<Position>(entity).Value;
        Position = new Vector2(position.X, position.Y);
    }
}
