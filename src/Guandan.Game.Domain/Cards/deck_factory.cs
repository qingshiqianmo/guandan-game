namespace Guandan.Game.Domain.Cards;

public static class DeckFactory
{
    public static IReadOnlyList<Card> CreateDoubleDeck()
    {
        List<Card> cards = new(108);
        int deckIndex = 0;

        while (deckIndex < 2)
        {
            AddStandardCards(cards, deckIndex);
            AddJokers(cards, deckIndex);
            deckIndex++;
        }

        return cards;
    }

    private static void AddStandardCards(ICollection<Card> cards, int deckIndex)
    {
        foreach (CardSuit suit in new[] { CardSuit.Clubs, CardSuit.Diamonds, CardSuit.Hearts, CardSuit.Spades })
        {
            for (int rankValue = (int)CardRank.Two; rankValue <= (int)CardRank.Ace; rankValue++)
            {
                string id = $"D{deckIndex}-{suit}-{rankValue}";
                cards.Add(new Card(id, suit, (CardRank)rankValue));
            }
        }
    }

    private static void AddJokers(ICollection<Card> cards, int deckIndex)
    {
        cards.Add(new Card($"D{deckIndex}-SJ", CardSuit.Joker, CardRank.SmallJoker));
        cards.Add(new Card($"D{deckIndex}-BJ", CardSuit.Joker, CardRank.BigJoker));
    }
}
