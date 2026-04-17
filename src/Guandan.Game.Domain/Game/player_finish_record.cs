using Guandan.Game.Domain.Players;

namespace Guandan.Game.Domain.Game;

public sealed record PlayerFinishRecord(PlayerSeat Seat, int Rank);
