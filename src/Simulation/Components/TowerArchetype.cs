namespace OpenTD.Simulation.Components;

public enum TowerArchetypeId
{
    Basic,
    Rapid,
    Slowing,
}

public readonly record struct TowerArchetype(TowerArchetypeId Id);
