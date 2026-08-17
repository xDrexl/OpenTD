using System.Numerics;
using OpenTD.Simulation.Components;

namespace OpenTD.Simulation.Commands;

public readonly record struct PlaceTower(
    Vector2 Position,
    TowerArchetypeId Archetype = TowerArchetypeId.Basic);
