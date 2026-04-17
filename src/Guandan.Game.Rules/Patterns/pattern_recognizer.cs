using Guandan.Game.Domain.Cards;
using Guandan.Game.Rules.Ranking;
using Guandan.Game.Rules.Wildcards;

namespace Guandan.Game.Rules.Patterns;

public sealed class PatternRecognizer
{
    private readonly CardOrderingService cardOrderingService;
    private readonly WildcardExpansionService wildcardExpansionService;

    public PatternRecognizer(
        CardOrderingService cardOrderingService,
        WildcardExpansionService wildcardExpansionService)
    {
        this.cardOrderingService = cardOrderingService;
        this.wildcardExpansionService = wildcardExpansionService;
    }

    public PatternDescriptor? Recognize(IReadOnlyList<Card> cards, GameRankContext context)
    {
        List<PatternDescriptor> descriptors = [];

        foreach (WildcardAssignment assignment in wildcardExpansionService.Expand(cards, context))
        {
            descriptors.AddRange(RecognizeAll(assignment.Cards, context));
        }

        return descriptors
            .OrderByDescending(item => item.Priority)
            .ThenByDescending(item => item.PrimaryStrength)
            .ThenByDescending(item => item.SequenceLength)
            .ThenByDescending(item => item.CardCount)
            .FirstOrDefault();
    }

    private IReadOnlyList<PatternDescriptor> RecognizeAll(
        IReadOnlyList<EffectiveCard> cards,
        GameRankContext context)
    {
        List<PatternDescriptor> results = [];
        AddSingle(cards, context, results);
        AddPair(cards, context, results);
        AddTriplet(cards, context, results);
        AddTripleWithPair(cards, context, results);
        AddStraight(cards, results);
        AddConsecutivePairs(cards, results);
        AddPlate(cards, results);
        AddBomb(cards, context, results);
        AddStraightFlush(cards, results);
        AddJokerBomb(cards, results);
        return results;
    }

    private void AddSingle(
        IReadOnlyList<EffectiveCard> cards,
        GameRankContext context,
        ICollection<PatternDescriptor> results)
    {
        if (cards.Count != 1)
        {
            return;
        }

        int strength = cardOrderingService.GetRankStrength(cards[0].Rank, context);
        results.Add(new PatternDescriptor(PatternType.Single, strength, 0, 1, 1, 10));
    }

    private void AddPair(
        IReadOnlyList<EffectiveCard> cards,
        GameRankContext context,
        ICollection<PatternDescriptor> results)
    {
        if (!TryGetSingleGroup(cards, 2, context, out int strength))
        {
            return;
        }

        results.Add(new PatternDescriptor(PatternType.Pair, strength, 0, 2, 1, 20));
    }

    private void AddTriplet(
        IReadOnlyList<EffectiveCard> cards,
        GameRankContext context,
        ICollection<PatternDescriptor> results)
    {
        if (!TryGetSingleGroup(cards, 3, context, out int strength))
        {
            return;
        }

        results.Add(new PatternDescriptor(PatternType.Triplet, strength, 0, 3, 1, 30));
    }

    private void AddTripleWithPair(
        IReadOnlyList<EffectiveCard> cards,
        GameRankContext context,
        ICollection<PatternDescriptor> results)
    {
        if (cards.Count != 5)
        {
            return;
        }

        List<IGrouping<int, EffectiveCard>> groups = GroupByStrength(cards, context);

        if (groups.Count != 2 || !groups.Any(group => group.Count() == 3) || !groups.Any(group => group.Count() == 2))
        {
            return;
        }

        int tripletStrength = groups.Single(group => group.Count() == 3).Key;
        int pairStrength = groups.Single(group => group.Count() == 2).Key;
        results.Add(new PatternDescriptor(PatternType.TripleWithPair, tripletStrength, pairStrength, 5, 1, 40));
    }

    private void AddStraight(IReadOnlyList<EffectiveCard> cards, ICollection<PatternDescriptor> results)
    {
        if (cards.Count < 5)
        {
            return;
        }

        if (!TryGetConsecutiveRanks(cards, 1, out IReadOnlyList<int> ranks))
        {
            return;
        }

        results.Add(new PatternDescriptor(PatternType.Straight, ranks.Max(), 0, cards.Count, ranks.Count, 50));
    }

    private void AddConsecutivePairs(IReadOnlyList<EffectiveCard> cards, ICollection<PatternDescriptor> results)
    {
        if (cards.Count < 6 || cards.Count % 2 != 0)
        {
            return;
        }

        if (!TryGetConsecutiveRanks(cards, 2, out IReadOnlyList<int> ranks))
        {
            return;
        }

        results.Add(new PatternDescriptor(PatternType.ConsecutivePairs, ranks.Max(), 0, cards.Count, ranks.Count, 45));
    }

    private void AddPlate(IReadOnlyList<EffectiveCard> cards, ICollection<PatternDescriptor> results)
    {
        if (cards.Count < 6 || cards.Count % 3 != 0)
        {
            return;
        }

        if (!TryGetConsecutiveRanks(cards, 3, out IReadOnlyList<int> ranks))
        {
            return;
        }

        results.Add(new PatternDescriptor(PatternType.Plate, ranks.Max(), 0, cards.Count, ranks.Count, 55));
    }

    private void AddBomb(
        IReadOnlyList<EffectiveCard> cards,
        GameRankContext context,
        ICollection<PatternDescriptor> results)
    {
        if (cards.Count < 4 || cards.Count == 4 && cards.All(card => card.Rank is CardRank.SmallJoker or CardRank.BigJoker))
        {
            return;
        }

        if (!TryGetSingleGroup(cards, cards.Count, context, out int strength))
        {
            return;
        }

        results.Add(new PatternDescriptor(PatternType.Bomb, strength, 0, cards.Count, 1, 80));
    }

    private void AddStraightFlush(IReadOnlyList<EffectiveCard> cards, ICollection<PatternDescriptor> results)
    {
        if (cards.Count < 5)
        {
            return;
        }

        if (!cards.Select(card => card.Suit).Distinct().Skip(1).Any() && TryGetConsecutiveRanks(cards, 1, out IReadOnlyList<int> ranks))
        {
            results.Add(new PatternDescriptor(PatternType.StraightFlush, ranks.Max(), 0, cards.Count, ranks.Count, 90));
        }
    }

    private void AddJokerBomb(IReadOnlyList<EffectiveCard> cards, ICollection<PatternDescriptor> results)
    {
        if (cards.Count != 4)
        {
            return;
        }

        if (cards.All(card => card.Rank is CardRank.SmallJoker or CardRank.BigJoker))
        {
            results.Add(new PatternDescriptor(PatternType.JokerBomb, 100, 0, 4, 1, 100));
        }
    }

    private List<IGrouping<int, EffectiveCard>> GroupByStrength(
        IReadOnlyList<EffectiveCard> cards,
        GameRankContext context)
    {
        return cards
            .GroupBy(card => cardOrderingService.GetRankStrength(card.Rank, context))
            .OrderBy(group => group.Key)
            .ToList();
    }

    private bool TryGetSingleGroup(
        IReadOnlyList<EffectiveCard> cards,
        int size,
        GameRankContext context,
        out int strength)
    {
        strength = 0;
        List<IGrouping<int, EffectiveCard>> groups = GroupByStrength(cards, context);

        if (groups.Count != 1 || groups[0].Count() != size)
        {
            return false;
        }

        strength = groups[0].Key;
        return true;
    }

    private bool TryGetConsecutiveRanks(
        IReadOnlyList<EffectiveCard> cards,
        int groupSize,
        out IReadOnlyList<int> ranks)
    {
        ranks = [];
        List<IGrouping<CardRank, EffectiveCard>> groups = cards.GroupBy(card => card.Rank).OrderBy(group => group.Key).ToList();

        if (groups.Any(group => group.Count() != groupSize))
        {
            return false;
        }

        List<int> rankValues = groups.Select(group => (int)group.Key).ToList();

        if (rankValues.Any(value => value <= (int)CardRank.Two || value >= (int)CardRank.SmallJoker))
        {
            return false;
        }

        for (int index = 1; index < rankValues.Count; index++)
        {
            if (rankValues[index] - rankValues[index - 1] != 1)
            {
                return false;
            }
        }

        ranks = rankValues;
        return true;
    }
}
