using Godot;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Presentation;

public sealed partial class EnemyView : Sprite2D
{
    private const float HitFlashSeconds = 0.12f;

    private SimulationWorld? _world;
    private Entity _entity;
    private int _previousHealth;
    private float _hitFlashRemaining;

    public void Initialize(SimulationWorld world, Entity entity)
    {
        _world = world;
        _entity = entity;
        if (world.TryGetComponent<Health>(entity, out var health))
        {
            _previousHealth = health.Current;
        }

        UpdatePosition();
    }

    public override void _Process(double delta)
    {
        if (_world is null)
        {
            return;
        }

        if (!_world.IsAlive(_entity))
        {
            if (GetParent() is { } parent)
            {
                ImpactEffect.Spawn(parent, Position, new Color(1, 0.3f, 0.2f));
            }

            QueueFree();
            return;
        }

        _hitFlashRemaining = Mathf.Max(0, _hitFlashRemaining - (float)delta);
        UpdatePosition();
        UpdateHealthFeedback();
    }

    private void UpdatePosition()
    {
        if (_world is not null && _world.TryGetComponent<Position>(_entity, out var position))
        {
            Position = new Vector2(position.Value.X, position.Value.Y);
        }
    }

    private void UpdateHealthFeedback()
    {
        if (_world is null ||
            !_world.TryGetComponent<Health>(_entity, out var health))
        {
            return;
        }

        if (health.Current < _previousHealth)
        {
            _hitFlashRemaining = HitFlashSeconds;
        }

        _previousHealth = health.Current;
        var isSlowed = _world.TryGetComponent<SlowEffect>(_entity, out _);
        Modulate = _hitFlashRemaining > 0
            ? new Color(1, 0.35f, 0.3f)
            : isSlowed
                ? new Color(0.55f, 0.85f, 1)
                : Colors.White;
        Scale = _hitFlashRemaining > 0 ? new Vector2(1.12f, 1.12f) : Vector2.One;
        QueueRedraw();
    }

    public override void _Draw()
    {
        if (_world is null ||
            !_world.TryGetComponent<Health>(_entity, out var health) ||
            health.Maximum <= 0)
        {
            return;
        }

        const float width = 30;
        const float height = 4;
        var ratio = Mathf.Clamp((float)health.Current / health.Maximum, 0, 1);
        var origin = new Vector2(-width / 2, -24);
        DrawRect(new Rect2(origin, new Vector2(width, height)), new Color(0.12f, 0.08f, 0.06f));
        DrawRect(new Rect2(origin, new Vector2(width * ratio, height)), new Color(0.3f, 0.95f, 0.3f));
    }
}
