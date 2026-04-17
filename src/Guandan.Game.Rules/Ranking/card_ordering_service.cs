using Guandan.Game.Domain.Cards;

namespace Guandan.Game.Rules.Ranking;

public sealed class CardOrderingService
{
    public bool IsWildcard(Card card, GameRankContext context)
    {
        return !card.IsJoker && card.Suit == CardSuit.Hearts && card.Rank == context.LevelRank;
    }

    public int GetCardStrength(Card card, GameRankContext context)
    {
        return GetRankStrength(card.Rank, context);
    }

    public int GetRankStrength(CardRank rank, GameRankContext context)
    {
        if (rank == CardRank.SmallJoker)
        {
            return 16;
        }

        if (rank == CardRank.BigJoker)
        {
            return 17;
        }

        if (rank == context.LevelRank)
        {
            return 15;
        }

        return (int)rank;
    }

    public IReadOnlyList<Card> SortCards(IEnumerable<Card> cards, GameRankContext context)
    {
        return cards
            .OrderBy(card => GetCardStrength(card, context))
            .ThenBy(card => card.Suit)
            .ToList();
    }
}
