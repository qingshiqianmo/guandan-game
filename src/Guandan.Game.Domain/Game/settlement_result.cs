using Guandan.Game.Domain.Players;

namespace Guandan.Game.Domain.Game;

public sealed record SettlementResult(
    IReadOnlyList<PlayerFinishRecord> FinishOrder,
    IReadOnlyList<PlayerSeat> WinningTeam,
    bool IsDoubleDown);
