using Godot;

namespace OpenTD.Presentation;

public sealed partial class ImpactEffect : Node2D
{
    private const float LifetimeSeconds = 0.25f;

    private Color _color = Colors.White;
    private float _elapsedSeconds;

    public static void Spawn(Node parent, Vector2 position, Color color)
    {
        var effect = new ImpactEffect
        {
            Position = position,
            _color = color,
        };
        parent.AddChild(effect);
    }

    public override void _Process(double delta)
    {
        _elapsedSeconds += (float)delta;
        if (_elapsedSeconds >= LifetimeSeconds)
        {
            QueueFree();
            return;
        }

        QueueRedraw();
    }

    public override void _Draw()
    {
        var progress = _elapsedSeconds / LifetimeSeconds;
        var color = new Color(_color, 1 - progress);
        DrawCircle(Vector2.Zero, Mathf.Lerp(4, 18, progress), color, false, 3, true);
    }
}
