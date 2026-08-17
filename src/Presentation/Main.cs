using Godot;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Systems;
using GameSimulation = OpenTD.Simulation.Simulation;
using NumericsVector2 = System.Numerics.Vector2;

namespace OpenTD.Presentation;

public sealed partial class Main : Node
{
    private static readonly NumericsVector2[] MapPath =
    [
        new(70, 324),
        new(224, 324),
        new(224, 176),
        new(480, 176),
        new(480, 456),
        new(736, 456),
        new(736, 192),
        new(1070, 192),
    ];

    private readonly GameSimulation _simulation = new(
    [
        new MovementSystem(),
        new PathCompletionSystem(),
        new BaseDamageSystem(),
    ]);

    public override void _Ready()
    {
        var baseEntity = _simulation.World.CreateEntity();
        _simulation.World.SetComponent(baseEntity, new Base());
        _simulation.World.SetComponent(baseEntity, new Health(20, 20));

        GetNode<BaseHealthView>("Interface/BaseHealth").Initialize(
            _simulation.World,
            baseEntity);

        var enemy = _simulation.World.CreateEntity();
        _simulation.World.SetComponent(enemy, new Enemy(1));
        _simulation.World.SetComponent(enemy, new Position(MapPath[0]));
        _simulation.World.SetComponent(enemy, new Movement(100));
        _simulation.World.SetComponent(enemy, new PathProgress(MapPath, 1));

        var enemyScene = GD.Load<PackedScene>("res://scenes/Enemy.tscn");
        var enemyView = enemyScene.Instantiate<EnemyView>();
        enemyView.Initialize(_simulation.World, enemy);
        AddChild(enemyView);
    }

    public override void _Process(double delta)
    {
        _simulation.Tick((float)delta);
    }
}
