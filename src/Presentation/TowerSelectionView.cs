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
        var slowing = configuration.GetDefinition(TowerArchetypeId.Slowing);
        var basicButton = GetNode<Button>("Basic");
        var rapidButton = GetNode<Button>("Rapid");
        var slowingButton = GetNode<Button>("Slowing");
        basicButton.Text = $"Basic ({basic.BuildCost})";
        rapidButton.Text = $"Rapid ({rapid.BuildCost})";
        slowingButton.Text = $"Slowing ({slowing.BuildCost})";
        basicButton.Pressed += () => Select(TowerArchetypeId.Basic);
        rapidButton.Pressed += () => Select(TowerArchetypeId.Rapid);
        slowingButton.Pressed += () => Select(TowerArchetypeId.Slowing);
        Select(selectedArchetype);
    }

    private void Select(TowerArchetypeId archetype)
    {
        GetNode<Button>("Basic").ButtonPressed = archetype == TowerArchetypeId.Basic;
        GetNode<Button>("Rapid").ButtonPressed = archetype == TowerArchetypeId.Rapid;
        GetNode<Button>("Slowing").ButtonPressed = archetype == TowerArchetypeId.Slowing;
        _selectionChanged?.Invoke(archetype);
    }
}
