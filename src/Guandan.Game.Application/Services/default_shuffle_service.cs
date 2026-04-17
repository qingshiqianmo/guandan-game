using Guandan.Game.Application.Abstractions;
using Guandan.Game.Domain.Cards;

namespace Guandan.Game.Application.Services;

public sealed class DefaultShuffleService : IShuffleService
{
    public IReadOnlyList<Card> Shuffle(IReadOnlyList<Card> deck)
    {
        Random random = Random.Shared;
        List<Card> shuffled = deck.ToList();

        for (int index = shuffled.Count - 1; index > 0; index--)
        {
            int target = random.Next(index + 1);
            (shuffled[index], shuffled[target]) = (shuffled[target], shuffled[index]);
        }

        return shuffled;
    }
}
