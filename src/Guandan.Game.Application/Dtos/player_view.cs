namespace Guandan.Game.Application.Dtos;

public sealed record PlayerView(
    int Seat,
    int CardCount,
    bool IsCurrentTurn,
    bool IsFinished,
    IReadOnlyList<CardView> Cards);
