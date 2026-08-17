namespace OpenTD.Simulation.Components;

public enum GamePhase
{
    Ready,
    Running,
    Victory,
    Defeat,
}

public readonly record struct GameStatus(GamePhase Phase);
