using Guandan.Game.Domain.Cards;
using Guandan.Game.Domain.Game;
using Guandan.Game.Domain.Players;

Run("双副牌数量正确", TestDeckFactory);
Run("玩家出牌后手牌减少", TestGameStatePlayFlow);
Console.WriteLine("Guandan.Game.Domain.Tests: PASS");

static void TestDeckFactory()
{
    IReadOnlyList<Card> deck = DeckFactory.CreateDoubleDeck();
    Assert(deck.Count == 108, "双副牌应为108张");
    Assert(deck.Count(card => card.IsJoker) == 4, "大小王总数应为4张");
}

static void TestGameStatePlayFlow()
{
    Dictionary<PlayerSeat, PlayerHand> hands = new()
    {
        [PlayerSeat.North] = new PlayerHand([new Card("n1", CardSuit.Clubs, CardRank.Three)]),
        [PlayerSeat.East] = new PlayerHand([new Card("e1", CardSuit.Clubs, CardRank.Four)]),
        [PlayerSeat.South] = new PlayerHand([new Card("s1", CardSuit.Clubs, CardRank.Five)]),
        [PlayerSeat.West] = new PlayerHand([new Card("w1", CardSuit.Clubs, CardRank.Six)]),
    };

    GameState gameState = GameState.Create(CardRank.Two, hands);
    gameState.ApplySuccessfulPlay(PlayerSeat.North, [hands[PlayerSeat.North].Cards[0]]);
    Assert(gameState.GetHand(PlayerSeat.North).Count == 0, "出牌后手牌应减少");
    Assert(gameState.IsFinished(PlayerSeat.North), "出完手牌后应记录名次");
}

static void Run(string name, Action test)
{
    try
    {
        test();
        Console.WriteLine($"[PASS] {name}");
    }
    catch (Exception exception)
    {
        Console.WriteLine($"[FAIL] {name}: {exception.Message}");
        Environment.Exit(1);
    }
}

static void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
