using Guandan.Game.Application.Abstractions;
using Guandan.Game.Domain.Game;

namespace Guandan.Game.Application.Services;

public sealed class InMemoryGameSessionStore : IGameSessionStore
{
    private GameState? gameState;

    public GameState? Load()
    {
        return gameState;
    }

    public void Save(GameState gameState)
    {
        this.gameState = gameState;
    }

    public void Clear()
    {
        gameState = null;
    }
}
