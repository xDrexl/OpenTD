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
    private readonly TowerPlacementConfiguration _towerPlacementConfiguration;
    private readonly WaveConfiguration _waveConfiguration;
    private TowerArchetypeId _selectedTowerArchetype = TowerArchetypeId.Basic;
    private Entity _gameStateEntity;
    private readonly Dictionary<Entity, EnemyView> _enemyViews = [];
    private readonly Dictionary<Entity, TowerView> _towerViews = [];
    private readonly Dictionary<Entity, ProjectileView> _projectileViews = [];

    public Main()
    {
        var map = MapConfiguration.CreateDefault();
        _waveConfiguration = WaveConfiguration.CreateDefault(map);
        _towerPlacementConfiguration = TowerPlacementConfiguration.Default;
        _towerPlacementSystem = new TowerPlacementSystem(
            map,
            _towerPlacementConfiguration);
        _simulation = new GameSimulation(
        [
            new WaveSystem(_waveConfiguration),
            new SlowSystem(),
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
            new GameStateSystem(),
        ]);
    }

    public override void _Ready()
    {
        _gameStateEntity = _simulation.World.CreateEntity();
        _simulation.World.SetComponent(
            _gameStateEntity,
            new GameStatus(GamePhase.Ready));
        GetNode<GameStateView>("Interface/GameState").Initialize(
            _simulation.World,
            _gameStateEntity);

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

        var waveEntity = _simulation.World.CreateEntity();
        var firstWaveEnemyCount = _waveConfiguration.Waves.Count == 0
            ? 0
            : _waveConfiguration.Waves[0].EnemyCount;
        _simulation.World.SetComponent(
            waveEntity,
            WaveState.Create(_waveConfiguration.Waves.Count, firstWaveEnemyCount));
        GetNode<WaveView>("Interface/Wave").Initialize(_simulation.World, waveEntity);
        GetNode<TowerSelectionView>("Interface/TowerSelection").Initialize(
            _towerPlacementConfiguration,
            _selectedTowerArchetype,
            SelectTower);
        ConfigurePlacementPreview();
    }

    public override void _Process(double delta)
    {
        UpdatePlacementPreview();
        if (!IsGameOver())
        {
            _simulation.Tick((float)delta);
        }

        SynchronizeEnemyViews();
        SynchronizeTowerViews();
        SynchronizeProjectileViews();
    }

    public override void _UnhandledInput(InputEvent @event)
    {
        if (GetGamePhase() == GamePhase.Running &&
            @event is InputEventMouseButton
            {
                ButtonIndex: MouseButton.Left,
                Pressed: true,
            })
        {
            var mousePosition = GetGlobalMousePosition();
            _simulation.World.EnqueueCommand(
                new PlaceTower(
                    new NumericsVector2(mousePosition.X, mousePosition.Y),
                    _selectedTowerArchetype));
        }
    }

    private void UpdatePlacementPreview()
    {
        var preview = GetNode<PlacementPreviewView>("PlacementPreview");
        preview.Visible = GetGamePhase() == GamePhase.Running;
        if (!preview.Visible)
        {
            return;
        }

        preview.Position = GetGlobalMousePosition();
        var position = new NumericsVector2(preview.Position.X, preview.Position.Y);
        preview.SetValidity(_towerPlacementSystem.CanPlace(
            _simulation.World,
            position,
            _selectedTowerArchetype));
    }

    private void SynchronizeTowerViews()
    {
        foreach (var entity in _simulation.World.Query<Tower, Position>())
        {
            if (_towerViews.ContainsKey(entity))
            {
                continue;
            }

            var archetype = _simulation.World.GetComponent<TowerArchetype>(entity).Id;
            var towerScene = GD.Load<PackedScene>(GetTowerScenePath(archetype));
            var towerView = towerScene.Instantiate<TowerView>();
            towerView.Initialize(_simulation.World, entity);
            AddChild(towerView);
            _towerViews.Add(entity, towerView);
        }
    }

    private void SynchronizeEnemyViews()
    {
        foreach (var entity in _enemyViews.Keys
                     .Where(entity => !_simulation.World.IsAlive(entity))
                     .ToArray())
        {
            _enemyViews.Remove(entity);
        }

        foreach (var entity in _simulation.World.Query<Enemy, Position>())
        {
            if (_enemyViews.ContainsKey(entity))
            {
                continue;
            }

            var archetype = _simulation.World.GetComponent<EnemyArchetype>(entity).Id;
            var enemyScene = GD.Load<PackedScene>(GetEnemyScenePath(archetype));
            var enemyView = enemyScene.Instantiate<EnemyView>();
            enemyView.Initialize(_simulation.World, entity);
            AddChild(enemyView);
            _enemyViews.Add(entity, enemyView);
        }
    }

    private static string GetEnemyScenePath(EnemyArchetypeId archetype) => archetype switch
    {
        EnemyArchetypeId.Basic => "res://scenes/Enemy.tscn",
        EnemyArchetypeId.Fast => "res://scenes/FastEnemy.tscn",
        _ => throw new System.ArgumentOutOfRangeException(nameof(archetype)),
    };

    private static string GetTowerScenePath(TowerArchetypeId archetype) => archetype switch
    {
        TowerArchetypeId.Basic => "res://scenes/Tower.tscn",
        TowerArchetypeId.Rapid => "res://scenes/RapidTower.tscn",
        TowerArchetypeId.Slowing => "res://scenes/SlowingTower.tscn",
        _ => throw new System.ArgumentOutOfRangeException(nameof(archetype)),
    };

    private void SelectTower(TowerArchetypeId archetype)
    {
        _selectedTowerArchetype = archetype;
        var preview = GetNode<PlacementPreviewView>("PlacementPreview");
        preview.Texture = GD.Load<Texture2D>(archetype switch
        {
            TowerArchetypeId.Basic => "res://assets/generated/tower.svg",
            TowerArchetypeId.Rapid => "res://assets/generated/rapid_tower.svg",
            TowerArchetypeId.Slowing => "res://assets/generated/slowing_tower.svg",
            _ => throw new System.ArgumentOutOfRangeException(nameof(archetype)),
        });
        ConfigurePlacementPreview();
    }

    private void ConfigurePlacementPreview()
    {
        GetNode<PlacementPreviewView>("PlacementPreview").Configure(
            _towerPlacementConfiguration.GetDefinition(_selectedTowerArchetype));
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

    private GamePhase GetGamePhase()
    {
        return _simulation.World.TryGetComponent<GameStatus>(
            _gameStateEntity,
            out var status)
            ? status.Phase
            : GamePhase.Ready;
    }

    private bool IsGameOver() => GetGamePhase() is GamePhase.Victory or GamePhase.Defeat;
}
