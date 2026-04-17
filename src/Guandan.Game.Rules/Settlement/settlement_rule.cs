using Guandan.Game.Domain.Game;
using Guandan.Game.Domain.Players;

namespace Guandan.Game.Rules.Settlement;

public sealed class SettlementRule
{
    public SettlementResult? TryBuild(GameState gameState)
    {
        IReadOnlyList<PlayerFinishRecord> finishOrder = gameState.FinishOrder;

        if (!TeamFinished(finishOrder, PlayerSeat.North, PlayerSeat.South) &&
            !TeamFinished(finishOrder, PlayerSeat.East, PlayerSeat.West))
        {
            return null;
        }

        IReadOnlyList<PlayerSeat> winningTeam = TeamFinished(finishOrder, PlayerSeat.North, PlayerSeat.South)
            ? [PlayerSeat.North, PlayerSeat.South]
            : [PlayerSeat.East, PlayerSeat.West];

        bool isDoubleDown = finishOrder.Count >= 2 &&
            winningTeam.Contains(finishOrder[0].Seat) &&
            winningTeam.Contains(finishOrder[1].Seat);

        return new SettlementResult(finishOrder.ToList(), winningTeam, isDoubleDown);
    }

    private static bool TeamFinished(
        IReadOnlyList<PlayerFinishRecord> finishOrder,
        PlayerSeat seatOne,
        PlayerSeat seatTwo)
    {
        HashSet<PlayerSeat> completed = finishOrder.Select(item => item.Seat).ToHashSet();
        return completed.Contains(seatOne) && completed.Contains(seatTwo);
    }
}
