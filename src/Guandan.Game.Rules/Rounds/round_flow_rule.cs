using Guandan.Game.Domain.Game;
using Guandan.Game.Domain.Players;

namespace Guandan.Game.Rules.Rounds;

public sealed class RoundFlowRule
{
    public PlayerSeat GetNextSeat(GameState gameState, PlayerSeat currentSeat)
    {
        PlayerSeat nextSeat = currentSeat;

        do
        {
            nextSeat = (PlayerSeat)(((int)nextSeat + 1) % 4);
        }
        while (gameState.IsFinished(nextSeat));

        return nextSeat;
    }

    public bool ShouldResetRound(GameState gameState)
    {
        int activePlayers = gameState.GetActiveSeats().Count;
        int requiredPasses = Math.Max(activePlayers - 1, 0);
        return requiredPasses > 0 && gameState.Round.ConsecutivePasses >= requiredPasses;
    }
}
