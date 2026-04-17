using Guandan.Game.Domain.Game;

namespace Guandan.Game.Application.Abstractions;

public interface IGameSessionStore
{
    GameState? Load();

    void Save(GameState gameState);

    void Clear();
}
