using Guandan.Game.Domain.Cards;

namespace Guandan.Game.Application.Abstractions;

public interface IShuffleService
{
    IReadOnlyList<Card> Shuffle(IReadOnlyList<Card> deck);
}
