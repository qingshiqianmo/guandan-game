using Guandan.Game.Domain.Cards;

namespace Guandan.Game.Rules.Ranking;

public sealed record EffectiveCard(
    string SourceCardId,
    CardSuit Suit,
    CardRank Rank,
    bool IsWildcardAssignment);
