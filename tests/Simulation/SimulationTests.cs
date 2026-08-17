using System;
using System.Collections.Generic;
using OpenTD.Simulation.Systems;
using Xunit;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Tests.Simulation;

public sealed class SimulationTests
{
    [Fact]
    public void TickExecutesSystemsInRegistrationOrder()
    {
        var executionOrder = new List<int>();
        var simulation = new OpenTD.Simulation.Simulation(
        [
            new RecordingSystem(1, executionOrder),
            new RecordingSystem(2, executionOrder),
        ]);

        simulation.Tick(0.25f);

        Assert.Equal([1, 2], executionOrder);
    }

    [Fact]
    public void TickPassesWorldAndDeltaTimeToSystems()
    {
        var system = new CapturingSystem();
        var simulation = new OpenTD.Simulation.Simulation([system]);

        simulation.Tick(0.25f);

        Assert.Same(simulation.World, system.World);
        Assert.Equal(0.25f, system.DeltaSeconds);
    }

    [Fact]
    public void TickRejectsNegativeDeltaTime()
    {
        var simulation = new OpenTD.Simulation.Simulation([]);

        Assert.Throws<ArgumentOutOfRangeException>(() => simulation.Tick(-0.01f));
    }

    private sealed class RecordingSystem(int id, List<int> executionOrder) : ISystem
    {
        public void Update(SimulationWorld world, float deltaSeconds) => executionOrder.Add(id);
    }

    private sealed class CapturingSystem : ISystem
    {
        public SimulationWorld? World { get; private set; }
        public float DeltaSeconds { get; private set; }

        public void Update(SimulationWorld world, float deltaSeconds)
        {
            World = world;
            DeltaSeconds = deltaSeconds;
        }
    }
}
