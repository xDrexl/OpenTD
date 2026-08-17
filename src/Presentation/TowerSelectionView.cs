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
        basicButton.TooltipText = $"Balanced tower — {basic.AttackDamage} damage, {basic.AttackRange:0} range";
        rapidButton.TooltipText = $"Fast attacks — {rapid.AttackDamage} damage, {rapid.AttackRange:0} range";
        slowingButton.TooltipText = $"Slows enemies — {slowing.AttackDamage} damage, {slowing.AttackRange:0} range";
        basicButton.Pressed += () => Select(TowerArchetypeId.Basic);
        rapidButton.Pressed += () => Select(TowerArchetypeId.Rapid);
        slowingButton.Pressed += () => Select(TowerArchetypeId.Slowing);
        Select(selectedArchetype);
    }

    private void Select(TowerArchetypeId archetype)
    {
        SetSelected(GetNode<Button>("Basic"), archetype == TowerArchetypeId.Basic);
        SetSelected(GetNode<Button>("Rapid"), archetype == TowerArchetypeId.Rapid);
        SetSelected(GetNode<Button>("Slowing"), archetype == TowerArchetypeId.Slowing);
        _selectionChanged?.Invoke(archetype);
    }

    private static void SetSelected(Button button, bool isSelected)
    {
        button.ButtonPressed = isSelected;
        button.Modulate = isSelected ? new Color(1, 0.9f, 0.45f) : Colors.White;
    }
}
