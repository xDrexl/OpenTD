using Godot;
using System.Collections.Generic;
using System.Linq;
using OpenTD.Simulation.Commands;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Configuration;
using OpenTD.Simulation.Systems;
using OpenTD.Simulation.World;
using GameSimulation = OpenTD.Simulation.Simulation;
using NumericsVector2 = System.Numerics.Vector2;

namespace OpenTD.Presentation;

public sealed partial class Main : Node2D
{
    private readonly GameSimulation _simulation;
    private readonly TowerPlacementSystem _towerPlacementSystem;
    private readonly Dictionary<Entity, TowerView> _towerViews = [];
    private readonly Dictionary<Entity, ProjectileView> _projectileViews = [];

    public Main()
    {
        var map = MapConfiguration.CreateDefault();
        _towerPlacementSystem = new TowerPlacementSystem(
            map,
            TowerPlacementConfiguration.Default);
        _simulation = new GameSimulation(
        [
            new MovementSystem(),
            new PathCompletionSystem(),
            new BaseDamageSystem(),
            _towerPlacementSystem,
            new TargetingSystem(),
            new AttackSystem(),
            new ProjectileSystem(),
            new DamageSystem(),
            new DeathSystem(),
            new EconomySystem(),
        ]);
        MapPath = map.Path;
    }

    private IReadOnlyList<NumericsVector2> MapPath { get; }

    public override void _Ready()
    {
        var baseEntity = _simulation.World.CreateEntity();
        _simulation.World.SetComponent(baseEntity, new Base());
        _simulation.World.SetComponent(baseEntity, new Health(20, 20));

        GetNode<BaseHealthView>("Interface/BaseHealth").Initialize(
            _simulation.World,
            baseEntity);

        var currencyEntity = _simulation.World.CreateEntity();
        _simulation.World.SetComponent(currencyEntity, new Currency(20));
        GetNode<CurrencyView>("Interface/Currency").Initialize(
            _simulation.World,
            currencyEntity);

        var enemy = _simulation.World.CreateEntity();
        _simulation.World.SetComponent(enemy, new Enemy(1));
        _simulation.World.SetComponent(enemy, new Health(10, 10));
        _simulation.World.SetComponent(enemy, new Reward(3));
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
        UpdatePlacementPreview();
        _simulation.Tick((float)delta);
        SynchronizeTowerViews();
        SynchronizeProjectileViews();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton
            {
                ButtonIndex: MouseButton.Left,
                Pressed: true,
            })
        {
            var mousePosition = GetGlobalMousePosition();
            _simulation.World.EnqueueCommand(
                new PlaceTower(new NumericsVector2(mousePosition.X, mousePosition.Y)));
        }
    }

    private void UpdatePlacementPreview()
    {
        var preview = GetNode<Sprite2D>("PlacementPreview");
        preview.Position = GetGlobalMousePosition();
        var position = new NumericsVector2(preview.Position.X, preview.Position.Y);
        preview.Modulate = _towerPlacementSystem.CanPlace(_simulation.World, position)
            ? new Color(0.55f, 1, 0.55f, 0.65f)
            : new Color(1, 0.4f, 0.4f, 0.65f);
    }

    private void SynchronizeTowerViews()
    {
        foreach (var entity in _simulation.World.Query<Tower, Position>())
        {
            if (_towerViews.ContainsKey(entity))
            {
                continue;
            }

            var towerScene = GD.Load<PackedScene>("res://scenes/Tower.tscn");
            var towerView = towerScene.Instantiate<TowerView>();
            towerView.Initialize(_simulation.World, entity);
            AddChild(towerView);
            _towerViews.Add(entity, towerView);
        }
    }

    private void SynchronizeProjectileViews()
    {
        foreach (var entity in _projectileViews.Keys
                     .Where(entity => !_simulation.World.IsAlive(entity))
                     .ToArray())
        {
            _projectileViews.Remove(entity);
        }

        foreach (var entity in _simulation.World.Query<Projectile, Position>())
        {
            if (_projectileViews.ContainsKey(entity))
            {
                continue;
            }

            var projectileScene = GD.Load<PackedScene>("res://scenes/Projectile.tscn");
            var projectileView = projectileScene.Instantiate<ProjectileView>();
            projectileView.Initialize(_simulation.World, entity);
            AddChild(projectileView);
            _projectileViews.Add(entity, projectileView);
        }
    }
}
