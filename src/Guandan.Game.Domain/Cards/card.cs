namespace Guandan.Game.Domain.Cards;

public sealed record Card(string Id, CardSuit Suit, CardRank Rank)
{
    public bool IsJoker => Suit == CardSuit.Joker;

    public string ToDisplayText()
    {
        if (Rank == CardRank.SmallJoker)
        {
            return "小王";
        }

        if (Rank == CardRank.BigJoker)
        {
            return "大王";
        }

        return $"{ToSuitText()}{ToRankText()}";
    }

    private string ToSuitText()
    {
        return Suit switch
        {
            CardSuit.Clubs => "梅花",
            CardSuit.Diamonds => "方块",
            CardSuit.Hearts => "红桃",
            CardSuit.Spades => "黑桃",
            _ => string.Empty,
        };
    }

    private string ToRankText()
    {
        return Rank switch
        {
            CardRank.Jack => "J",
            CardRank.Queen => "Q",
            CardRank.King => "K",
            CardRank.Ace => "A",
            _ => ((int)Rank).ToString(),
        };
    }
}
