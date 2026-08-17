using System;
using System.Collections.Generic;
using System.Linq;
using OpenTD.Simulation.Systems;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation;

public sealed class Simulation
{
    private readonly IReadOnlyList<ISystem> _systems;

    public Simulation(IEnumerable<ISystem> systems)
    {
        _systems = systems.ToArray();
    }

    public SimulationWorld World { get; } = new();

    public void Tick(float deltaSeconds)
    {
        if (deltaSeconds < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(deltaSeconds));
        }

        foreach (var system in _systems)
        {
            system.Update(World, deltaSeconds);
        }
    }
}
