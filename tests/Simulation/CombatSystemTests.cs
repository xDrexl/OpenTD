using System.Numerics;
using OpenTD.Simulation.Components;
using OpenTD.Simulation.Events;
using OpenTD.Simulation.Systems;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class CombatSystemTests
{
    [Fact]
    public void ReadyTowerCreatesProjectileAndResetsCooldown()
    {
        var world = new SimulationWorld();
        var enemy = CreateEnemy(world, new Vector2(10, 0), 10);
        var tower = CreateTower(world, Vector2.Zero, enemy);

        new AttackSystem().Update(world, 0.1f);

        var projectile = Assert.Single(world.Query<Projectile, Position>());
        Assert.Equal(enemy, world.GetComponent<Projectile>(projectile).Target);
        Assert.Equal(Vector2.Zero, world.GetComponent<Position>(projectile).Value);
        Assert.Equal(2, world.GetComponent<Damage>(projectile).Amount);
        Assert.Equal(1, world.GetComponent<AttackCooldown>(tower).RemainingSeconds);
    }

    [Fact]
    public void TowerDoesNotAttackUntilCooldownExpires()
    {
        var world = new SimulationWorld();
        var enemy = CreateEnemy(world, new Vector2(10, 0), 10);
        var tower = CreateTower(world, Vector2.Zero, enemy);
        world.SetComponent(tower, new AttackCooldown(1, 0.5f));
        var system = new AttackSystem();

        system.Update(world, 0.25f);
        Assert.Empty(world.Query<Projectile>());
        system.Update(world, 0.25f);

        Assert.Single(world.Query<Projectile>());
    }

    [Fact]
    public void ProjectileMovesTowardTargetBeforeImpact()
    {
        var world = new SimulationWorld();
        var enemy = CreateEnemy(world, new Vector2(10, 0), 10);
        var projectile = CreateProjectile(world, Vector2.Zero, enemy, 4, 2);

        new ProjectileSystem().Update(world, 0.5f);

        Assert.Equal(new Vector2(2, 0), world.GetComponent<Position>(projectile).Value);
        Assert.Empty(world.DrainEvents<DamageRequested>());
    }

    [Fact]
    public void ProjectileImpactRequestsDamageAndRemovesProjectile()
    {
        var world = new SimulationWorld();
        var enemy = CreateEnemy(world, new Vector2(3, 0), 10);
        var projectile = CreateProjectile(world, Vector2.Zero, enemy, 4, 2);

        new ProjectileSystem().Update(world, 1);

        Assert.False(world.IsAlive(projectile));
        var damage = Assert.Single(world.DrainEvents<DamageRequested>());
        Assert.Equal(enemy, damage.Target);
        Assert.Equal(2, damage.Amount);
    }

    [Fact]
    public void DamageAndDeathSystemsKillEnemyAndEmitDeath()
    {
        var world = new SimulationWorld();
        var enemy = CreateEnemy(world, Vector2.Zero, 2);
        world.Emit(new DamageRequested(enemy, 3));

        new DamageSystem().Update(world, 0);
        Assert.Equal(0, world.GetComponent<Health>(enemy).Current);
        new DeathSystem().Update(world, 0);

        Assert.False(world.IsAlive(enemy));
        Assert.Equal(enemy, Assert.Single(world.DrainEvents<EnemyDied>()).Enemy);
    }

    [Fact]
    public void ProjectileIsRemovedWhenTargetNoLongerExists()
    {
        var world = new SimulationWorld();
        var enemy = CreateEnemy(world, new Vector2(3, 0), 10);
        var projectile = CreateProjectile(world, Vector2.Zero, enemy, 4, 2);
        world.DestroyEntity(enemy);

        new ProjectileSystem().Update(world, 1);

        Assert.False(world.IsAlive(projectile));
        Assert.Empty(world.DrainEvents<DamageRequested>());
    }

    private static Entity CreateTower(SimulationWorld world, Vector2 position, Entity target)
    {
        var tower = world.CreateEntity();
        world.SetComponent(tower, new Position(position));
        world.SetComponent(tower, new Target(target));
        world.SetComponent(tower, new AttackCooldown(1, 0));
        world.SetComponent(tower, new AttackStats(2, 4));
        return tower;
    }

    private static Entity CreateEnemy(
        SimulationWorld world,
        Vector2 position,
        int healthAmount)
    {
        var enemy = world.CreateEntity();
        world.SetComponent(enemy, new Enemy(1));
        world.SetComponent(enemy, new Position(position));
        world.SetComponent(enemy, new Health(healthAmount, healthAmount));
        return enemy;
    }

    private static Entity CreateProjectile(
        SimulationWorld world,
        Vector2 position,
        Entity target,
        float speed,
        int damage)
    {
        var projectile = world.CreateEntity();
        world.SetComponent(projectile, new Position(position));
        world.SetComponent(projectile, new Projectile(target, speed));
        world.SetComponent(projectile, new Damage(damage));
        return projectile;
    }
}
