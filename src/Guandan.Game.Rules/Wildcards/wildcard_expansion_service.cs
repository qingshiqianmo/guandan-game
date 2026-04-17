using Guandan.Game.Domain.Cards;
using Guandan.Game.Rules.Ranking;

namespace Guandan.Game.Rules.Wildcards;

public sealed class WildcardExpansionService
{
    private static readonly CardSuit[] StandardSuits =
        [CardSuit.Clubs, CardSuit.Diamonds, CardSuit.Hearts, CardSuit.Spades];

    private readonly CardOrderingService cardOrderingService;

    public WildcardExpansionService(CardOrderingService cardOrderingService)
    {
        this.cardOrderingService = cardOrderingService;
    }

    public IReadOnlyList<WildcardAssignment> Expand(
        IReadOnlyList<Card> cards,
        GameRankContext context)
    {
        List<Card> wildcards = cards.Where(card => cardOrderingService.IsWildcard(card, context)).ToList();
        List<Card> normalCards = cards.Where(card => !cardOrderingService.IsWildcard(card, context)).ToList();

        if (wildcards.Count == 0)
        {
            return [new WildcardAssignment(normalCards.Select(ToEffectiveCard).ToList())];
        }

        List<EffectiveCard> fixedCards = normalCards.Select(ToEffectiveCard).ToList();
        List<WildcardAssignment> results = [];
        BuildAssignments(fixedCards, wildcards, 0, [], results);
        return results;
    }

    private static EffectiveCard ToEffectiveCard(Card card)
    {
        return new EffectiveCard(card.Id, card.Suit, card.Rank, false);
    }

    private void BuildAssignments(
        IReadOnlyList<EffectiveCard> fixedCards,
        IReadOnlyList<Card> wildcards,
        int index,
        IReadOnlyList<EffectiveCard> current,
        ICollection<WildcardAssignment> results)
    {
        if (index >= wildcards.Count)
        {
            results.Add(new WildcardAssignment(fixedCards.Concat(current).ToList()));
            return;
        }

        foreach (CardRank rank in Enum.GetValues<CardRank>())
        {
            if (rank == CardRank.SmallJoker || rank == CardRank.BigJoker)
            {
                continue;
            }

            foreach (CardSuit suit in StandardSuits)
            {
                EffectiveCard effectiveCard = new(wildcards[index].Id, suit, rank, true);
                List<EffectiveCard> next = current.Append(effectiveCard).ToList();
                BuildAssignments(fixedCards, wildcards, index + 1, next, results);
            }
        }
    }
}
