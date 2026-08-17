using OpenTD.Simulation.World;

namespace OpenTD.Simulation.Components;

public readonly record struct Projectile(Entity Target, float Speed);
