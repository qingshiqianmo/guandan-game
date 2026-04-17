namespace Guandan.Game.Application.Commands;

public sealed record PlayCardsCommand(int Seat, IReadOnlyList<string> CardIds);
