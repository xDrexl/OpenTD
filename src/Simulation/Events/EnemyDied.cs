using OpenTD.Simulation.World;

namespace OpenTD.Simulation.Events;

public readonly record struct EnemyDied(Entity Enemy, int Reward);
