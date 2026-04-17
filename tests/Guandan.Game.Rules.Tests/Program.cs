using Guandan.Game.Domain.Cards;
using Guandan.Game.Domain.Game;
using Guandan.Game.Domain.Players;
using Guandan.Game.Rules.Patterns;
using Guandan.Game.Rules.Ranking;
using Guandan.Game.Rules.Rounds;
using Guandan.Game.Rules.Validation;
using Guandan.Game.Rules.Wildcards;

CardOrderingService cardOrderingService = new();
WildcardExpansionService wildcardExpansionService = new(cardOrderingService);
PatternRecognizer patternRecognizer = new(cardOrderingService, wildcardExpansionService);
PlayComparisonService playComparisonService = new();
PlayValidationService playValidationService = new(cardOrderingService, patternRecognizer, playComparisonService);
RoundFlowRule roundFlowRule = new();

Run("顺子识别正确", TestStraightRecognition);
Run("红桃级牌可补对子", TestWildcardPairRecognition);
Run("炸弹可压普通牌型", TestBombComparison);
Run("三家不跟后重置牌权", TestRoundReset);
Console.WriteLine("Guandan.Game.Rules.Tests: PASS");

void TestStraightRecognition()
{
    PatternDescriptor? pattern = patternRecognizer.Recognize(
        [
            new Card("1", CardSuit.Clubs, CardRank.Three),
            new Card("2", CardSuit.Diamonds, CardRank.Four),
            new Card("3", CardSuit.Hearts, CardRank.Five),
            new Card("4", CardSuit.Spades, CardRank.Six),
            new Card("5", CardSuit.Clubs, CardRank.Seven),
        ],
        GameRankContext.Default);

    Assert(pattern?.Type == PatternType.Straight, "应识别为顺子");
}

void TestWildcardPairRecognition()
{
    PatternDescriptor? pattern = patternRecognizer.Recognize(
        [
            new Card("1", CardSuit.Hearts, CardRank.Two),
            new Card("2", CardSuit.Clubs, CardRank.Ace),
        ],
        GameRankContext.Default);

    Assert(pattern?.Type == PatternType.Pair, "红桃2应补成对子");
}

void TestBombComparison()
{
    PatternDescriptor bomb = new(PatternType.Bomb, 9, 0, 4, 1, 80);
    PatternDescriptor straight = new(PatternType.Straight, 7, 0, 5, 5, 50);
    Assert(playComparisonService.CanBeat(bomb, straight), "炸弹应压顺子");
}

void TestRoundReset()
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
    gameState.MoveTurnTo(PlayerSeat.East);
    gameState.ApplyPass(PlayerSeat.East);
    gameState.MoveTurnTo(PlayerSeat.South);
    gameState.ApplyPass(PlayerSeat.South);
    gameState.MoveTurnTo(PlayerSeat.West);
    gameState.ApplyPass(PlayerSeat.West);

    Assert(roundFlowRule.ShouldResetRound(gameState), "三家不跟后应重置牌权");
}

void Run(string name, Action test)
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

void Assert(bool condition, string message)
{
    if (!condition)
    {
        throw new InvalidOperationException(message);
    }
}
