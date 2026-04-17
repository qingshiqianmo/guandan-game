using Guandan.Game.Domain.Cards;
using Guandan.Game.Domain.Players;

namespace Guandan.Game.Domain.Game;

public sealed record PlayedCombo(PlayerSeat Seat, IReadOnlyList<Card> Cards);
