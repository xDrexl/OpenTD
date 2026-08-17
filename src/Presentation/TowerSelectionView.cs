using Godot;
using System;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Configuration;

namespace OpenTD.Presentation;

public sealed partial class TowerSelectionView : HBoxContainer
{
    private Action<TowerArchetypeId>? _selectionChanged;

    public void Initialize(
        TowerPlacementConfiguration configuration,
        TowerArchetypeId selectedArchetype,
        Action<TowerArchetypeId> selectionChanged)
    {
        _selectionChanged = selectionChanged;
        var basic = configuration.GetDefinition(TowerArchetypeId.Basic);
        var rapid = configuration.GetDefinition(TowerArchetypeId.Rapid);
        var basicButton = GetNode<Button>("Basic");
        var rapidButton = GetNode<Button>("Rapid");
        basicButton.Text = $"Basic ({basic.BuildCost})";
        rapidButton.Text = $"Rapid ({rapid.BuildCost})";
        basicButton.Pressed += () => Select(TowerArchetypeId.Basic);
        rapidButton.Pressed += () => Select(TowerArchetypeId.Rapid);
        Select(selectedArchetype);
    }

    private void Select(TowerArchetypeId archetype)
    {
        GetNode<Button>("Basic").ButtonPressed = archetype == TowerArchetypeId.Basic;
        GetNode<Button>("Rapid").ButtonPressed = archetype == TowerArchetypeId.Rapid;
        _selectionChanged?.Invoke(archetype);
    }
}
