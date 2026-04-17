namespace Guandan.Game.Application.Dtos;

public sealed record PlayedComboView(int Seat, IReadOnlyList<CardView> Cards);
