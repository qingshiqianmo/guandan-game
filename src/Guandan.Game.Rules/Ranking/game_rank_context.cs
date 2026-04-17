using Guandan.Game.Domain.Cards;

namespace Guandan.Game.Rules.Ranking;

public sealed record GameRankContext(CardRank LevelRank)
{
    public static GameRankContext Default { get; } = new(CardRank.Two);
}
