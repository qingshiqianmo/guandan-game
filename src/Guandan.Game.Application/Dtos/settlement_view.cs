namespace Guandan.Game.Application.Dtos;

public sealed record SettlementView(
    IReadOnlyList<int> WinningTeam,
    bool IsDoubleDown,
    IReadOnlyList<string> FinishOrder);
