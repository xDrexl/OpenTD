using System.Linq;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Events;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class DeathSystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        foreach (var enemy in world.Query<Enemy, Health>().ToArray())
        {
            if (world.GetComponent<Health>(enemy).Current > 0)
            {
                continue;
            }

            world.Emit(new EnemyDied(enemy));
            world.DestroyEntity(enemy);
        }
    }
}
