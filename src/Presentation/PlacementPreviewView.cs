using Godot;
using OpenTD.Simulation.Configuration;

namespace OpenTD.Presentation;

public sealed partial class PlacementPreviewView : Sprite2D
{
    private float _attackRange;
    private float _placementRadius;
    private bool _isValid;

    public void Configure(TowerDefinition definition)
    {
        _attackRange = definition.AttackRange;
        _placementRadius = definition.PlacementRadius;
        QueueRedraw();
    }

    public void SetValidity(bool isValid)
    {
        _isValid = isValid;
        Modulate = isValid
            ? new Color(0.55f, 1, 0.55f, 0.75f)
            : new Color(1, 0.4f, 0.4f, 0.75f);
        QueueRedraw();
    }

    public override void _Draw()
    {
        var color = _isValid
            ? new Color(0.55f, 1, 0.55f, 0.45f)
            : new Color(1, 0.4f, 0.4f, 0.45f);
        DrawCircle(Vector2.Zero, _attackRange, color, false, 2, true);
        DrawCircle(Vector2.Zero, _placementRadius, color, false, 2, true);
    }
}
