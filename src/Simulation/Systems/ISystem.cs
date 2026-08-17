using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public interface ISystem
{
    void Update(SimulationWorld world, float deltaSeconds);
}
