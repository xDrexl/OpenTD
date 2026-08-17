using OpenTD.Simulation.World;

namespace OpenTD.Simulation.Events;

public readonly record struct EnemyArrived(Entity Enemy, int Damage);
