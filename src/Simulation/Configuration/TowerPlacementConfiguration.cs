using System.Collections.Generic;
using System.Linq;
using OpenTD.Simulation.Components;

namespace OpenTD.Simulation.Configuration;

public sealed record TowerPlacementConfiguration(IReadOnlyList<TowerDefinition> Towers)
{
    public static TowerPlacementConfiguration Default { get; } = new(
        new TowerDefinition[]
        {
            new(TowerArchetypeId.Basic, 5, 28, 150, 0.75f, 2, 250),
            new(TowerArchetypeId.Rapid, 7, 24, 125, 0.3f, 1, 320),
            new(
                TowerArchetypeId.Slowing,
                8,
                26,
                135,
                1.2f,
                1,
                220,
                new SlowDefinition(0.55f, 2.5f)),
        });

    public bool TryGetDefinition(TowerArchetypeId archetype, out TowerDefinition definition)
    {
        definition = Towers.FirstOrDefault(tower => tower.Archetype == archetype)!;
        return definition is not null;
    }

    public TowerDefinition GetDefinition(TowerArchetypeId archetype)
    {
        return TryGetDefinition(archetype, out var definition)
            ? definition
            : throw new KeyNotFoundException($"Unknown tower archetype: {archetype}.");
    }
}
