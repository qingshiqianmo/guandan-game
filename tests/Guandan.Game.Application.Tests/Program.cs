using Guandan.Game.Application.Abstractions;
using Guandan.Game.Application.Commands;
using Guandan.Game.Application.Services;
using Guandan.Game.Domain.Cards;
using Guandan.Game.Rules.Patterns;
using Guandan.Game.Rules.Ranking;
using Guandan.Game.Rules.Rounds;
using Guandan.Game.Rules.Settlement;
using Guandan.Game.Rules.Validation;
using Guandan.Game.Rules.Wildcards;

Run("开局后每家27张", TestStartGame);
Run("出牌后轮到下一家", TestPlayCardsFlow);
Console.WriteLine("Guandan.Game.Application.Tests: PASS");

void TestStartGame()
{
    GameApplicationService service = CreateService();
    var game = service.StartGame(new StartGameCommand());
    Assert(game.Players.All(player => player.CardCount == 27), "开局后每家应有27张");
}

void TestPlayCardsFlow()
{
    GameApplicationService service = CreateService();
    var game = service.StartGame(new StartGameCommand());
    string cardId = game.Players.First(player => player.Seat == 0).Cards[0].Id;
    var result = service.PlayCards(new PlayCardsCommand(0, [cardId]));
    Assert(result.IsSuccess, "首家出单张应成功");
    Assert(result.Game?.CurrentTurn == 1, "出牌后应轮到下一家");
}

GameApplicationService CreateService()
{
    CardOrderingService cardOrderingService = new();
    WildcardExpansionService wildcardExpansionService = new(cardOrderingService);
    PatternRecognizer patternRecognizer = new(cardOrderingService, wildcardExpansionService);
    PlayComparisonService playComparisonService = new();
    PlayValidationService playValidationService = new(cardOrderingService, patternRecognizer, playComparisonService);

    return new GameApplicationService(
        new InMemoryGameSessionStore(),
        new FixedShuffleService(),
        cardOrderingService,
        patternRecognizer,
        playValidationService,
        new RoundFlowRule(),
        new SettlementRule());
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

sealed class FixedShuffleService : IShuffleService
{
    public IReadOnlyList<Card> Shuffle(IReadOnlyList<Card> deck)
    {
        return deck;
    }
}
