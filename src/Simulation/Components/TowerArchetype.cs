namespace OpenTD.Simulation.Components;

public enum TowerArchetypeId
{
    Basic,
    Rapid,
}

public readonly record struct TowerArchetype(TowerArchetypeId Id);
