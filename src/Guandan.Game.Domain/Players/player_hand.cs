using Guandan.Game.Domain.Cards;

namespace Guandan.Game.Domain.Players;

public sealed class PlayerHand
{
    private readonly List<Card> cards;

    public PlayerHand(IEnumerable<Card> sourceCards)
    {
        cards = sourceCards.ToList();
    }

    public IReadOnlyList<Card> Cards => cards;

    public int Count => cards.Count;

    public bool ContainsAll(IEnumerable<string> cardIds)
    {
        HashSet<string> required = cardIds.ToHashSet(StringComparer.Ordinal);
        HashSet<string> owned = cards.Select(card => card.Id).ToHashSet(StringComparer.Ordinal);
        return required.All(owned.Contains);
    }

    public IReadOnlyList<Card> FindByIds(IEnumerable<string> cardIds)
    {
        HashSet<string> ids = cardIds.ToHashSet(StringComparer.Ordinal);
        return cards.Where(card => ids.Contains(card.Id)).ToList();
    }

    public void RemoveByIds(IEnumerable<string> cardIds)
    {
        HashSet<string> ids = cardIds.ToHashSet(StringComparer.Ordinal);
        cards.RemoveAll(card => ids.Contains(card.Id));
    }
}
