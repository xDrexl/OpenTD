using System.Linq;
using OpenTD.Simulation.Components;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class PathCompletionSystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        foreach (var entity in world.Query<PathProgress>().ToArray())
        {
            var progress = world.GetComponent<PathProgress>(entity);
            if (!progress.IsCompleted && progress.NextWaypointIndex >= progress.Waypoints.Count)
            {
                world.SetComponent(entity, progress with { IsCompleted = true });
            }
        }
    }
}
