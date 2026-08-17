using OpenTD.Simulation.World;

namespace OpenTD.Simulation.Events;

public readonly record struct DamageRequested(Entity Target, int Amount);
