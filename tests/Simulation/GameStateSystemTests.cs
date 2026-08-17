using OpenTD.Simulation.Components;
using OpenTD.Simulation.Systems;
using OpenTD.Simulation.World;
using SimulationWorld = OpenTD.Simulation.World.World;
using Xunit;

namespace OpenTD.Tests.Simulation;

public sealed class GameStateSystemTests
{
    [Fact]
    public void ReadyGameBeginsRunning()
    {
        var (world, statusEntity) = CreateWorld(GamePhase.Ready);

        new GameStateSystem().Update(world, 0);

        Assert.Equal(GamePhase.Running, world.GetComponent<GameStatus>(statusEntity).Phase);
    }

    [Fact]
    public void ZeroBaseHealthCausesDefeat()
    {
        var (world, statusEntity) = CreateWorld(GamePhase.Running);
        var baseEntity = world.CreateEntity();
        world.SetComponent(baseEntity, new Base());
        world.SetComponent(baseEntity, new Health(0, 20));

        new GameStateSystem().Update(world, 0);

        Assert.Equal(GamePhase.Defeat, world.GetComponent<GameStatus>(statusEntity).Phase);
    }

    [Fact]
    public void CompletedWavesWithoutEnemiesCauseVictory()
    {
        var (world, statusEntity) = CreateWorld(GamePhase.Running);
        var waveEntity = world.CreateEntity();
        world.SetComponent(
            waveEntity,
            new WaveState(1, 1, 1, 1, 0, 0, false, true));

        new GameStateSystem().Update(world, 0);

        Assert.Equal(GamePhase.Victory, world.GetComponent<GameStatus>(statusEntity).Phase);
    }

    [Fact]
    public void ActiveEnemyPreventsVictory()
    {
        var (world, statusEntity) = CreateWorld(GamePhase.Running);
        var waveEntity = world.CreateEntity();
        world.SetComponent(
            waveEntity,
            new WaveState(1, 1, 1, 1, 1, 0, false, true));
        var enemy = world.CreateEntity();
        world.SetComponent(enemy, new Enemy(1));

        new GameStateSystem().Update(world, 0);

        Assert.Equal(GamePhase.Running, world.GetComponent<GameStatus>(statusEntity).Phase);
    }

    [Theory]
    [InlineData(GamePhase.Victory)]
    [InlineData(GamePhase.Defeat)]
    public void TerminalStateDoesNotTransition(GamePhase terminalPhase)
    {
        var (world, statusEntity) = CreateWorld(terminalPhase);

        new GameStateSystem().Update(world, 0);

        Assert.Equal(terminalPhase, world.GetComponent<GameStatus>(statusEntity).Phase);
    }

    private static (SimulationWorld World, Entity StatusEntity) CreateWorld(
        GamePhase phase)
    {
        var world = new SimulationWorld();
        var statusEntity = world.CreateEntity();
        world.SetComponent(statusEntity, new GameStatus(phase));
        return (world, statusEntity);
    }
}
