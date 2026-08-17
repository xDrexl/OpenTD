using System;
using System.Linq;
using OpenTD.Simulation.Components;
using SimulationWorld = OpenTD.Simulation.World.World;

namespace OpenTD.Simulation.Systems;

public sealed class GameStateSystem : ISystem
{
    public void Update(SimulationWorld world, float deltaSeconds)
    {
        var gameStateEntities = world.Query<GameStatus>().Take(2).ToArray();
        if (gameStateEntities.Length != 1)
        {
            throw new InvalidOperationException("Exactly one game status entity is required.");
        }

        var gameStateEntity = gameStateEntities[0];
        var status = world.GetComponent<GameStatus>(gameStateEntity);
        if (status.Phase == GamePhase.Ready)
        {
            world.SetComponent(gameStateEntity, new GameStatus(GamePhase.Running));
            return;
        }

        if (status.Phase != GamePhase.Running)
        {
            return;
        }

        if (world.Query<Base, Health>()
            .Any(entity => world.GetComponent<Health>(entity).Current <= 0))
        {
            world.SetComponent(gameStateEntity, new GameStatus(GamePhase.Defeat));
            return;
        }

        var wavesComplete = world.Query<WaveState>()
            .Any(entity => world.GetComponent<WaveState>(entity).IsComplete);
        if (wavesComplete && !world.Query<Enemy>().Any())
        {
            world.SetComponent(gameStateEntity, new GameStatus(GamePhase.Victory));
        }
    }
}
