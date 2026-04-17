using Guandan.Game.Application.Abstractions;
using Guandan.Game.Application.Commands;
using Guandan.Game.Application.Dtos;
using Guandan.Game.Domain.Cards;
using Guandan.Game.Domain.Game;
using Guandan.Game.Domain.Players;
using Guandan.Game.Rules.Patterns;
using Guandan.Game.Rules.Ranking;
using Guandan.Game.Rules.Rounds;
using Guandan.Game.Rules.Settlement;
using Guandan.Game.Rules.Validation;

namespace Guandan.Game.Application.Services;

public sealed class GameApplicationService
{
    private readonly IGameSessionStore gameSessionStore;
    private readonly IShuffleService shuffleService;
    private readonly CardOrderingService cardOrderingService;
    private readonly PatternRecognizer patternRecognizer;
    private readonly PlayValidationService playValidationService;
    private readonly RoundFlowRule roundFlowRule;
    private readonly SettlementRule settlementRule;

    public GameApplicationService(
        IGameSessionStore gameSessionStore,
        IShuffleService shuffleService,
        CardOrderingService cardOrderingService,
        PatternRecognizer patternRecognizer,
        PlayValidationService playValidationService,
        RoundFlowRule roundFlowRule,
        SettlementRule settlementRule)
    {
        this.gameSessionStore = gameSessionStore;
        this.shuffleService = shuffleService;
        this.cardOrderingService = cardOrderingService;
        this.patternRecognizer = patternRecognizer;
        this.playValidationService = playValidationService;
        this.roundFlowRule = roundFlowRule;
        this.settlementRule = settlementRule;
    }

    public GameView StartGame(StartGameCommand _)
    {
        IReadOnlyList<Card> deck = shuffleService.Shuffle(DeckFactory.CreateDoubleDeck());
        Dictionary<PlayerSeat, PlayerHand> hands = DealHands(deck);
        GameState gameState = GameState.Create(CardRank.Two, hands);
        gameSessionStore.Save(gameState);
        return ToGameView(gameState);
    }

    public ActionResult PlayCards(PlayCardsCommand command)
    {
        GameState? gameState = gameSessionStore.Load();

        if (gameState is null)
        {
            return ActionResult.Failure("game_not_started", null);
        }

        PlayerSeat seat = (PlayerSeat)command.Seat;
        PlayValidationResult validation = playValidationService.ValidatePlay(gameState, seat, command.CardIds);

        if (!validation.IsValid)
        {
            return ActionResult.Failure(validation.ErrorCode!, ToGameView(gameState));
        }

        gameState.ApplySuccessfulPlay(seat, validation.SortedCards!);
        AdvanceAfterPlay(gameState, seat);
        gameSessionStore.Save(gameState);
        return ActionResult.Success(ToGameView(gameState));
    }

    public ActionResult PassTurn(PassTurnCommand command)
    {
        GameState? gameState = gameSessionStore.Load();

        if (gameState is null)
        {
            return ActionResult.Failure("game_not_started", null);
        }

        PlayerSeat seat = (PlayerSeat)command.Seat;
        string? errorCode = playValidationService.ValidatePass(gameState, seat);

        if (errorCode is not null)
        {
            return ActionResult.Failure(errorCode, ToGameView(gameState));
        }

        gameState.ApplyPass(seat);
        AdvanceAfterPass(gameState, seat);
        gameSessionStore.Save(gameState);
        return ActionResult.Success(ToGameView(gameState));
    }

    public GameView? GetGameState()
    {
        return gameSessionStore.Load() is { } gameState ? ToGameView(gameState) : null;
    }

    public GameView RestartGame()
    {
        gameSessionStore.Clear();
        return StartGame(new StartGameCommand());
    }

    private Dictionary<PlayerSeat, PlayerHand> DealHands(IReadOnlyList<Card> deck)
    {
        return new Dictionary<PlayerSeat, PlayerHand>
        {
            [PlayerSeat.North] = new PlayerHand(deck.Take(27)),
            [PlayerSeat.East] = new PlayerHand(deck.Skip(27).Take(27)),
            [PlayerSeat.South] = new PlayerHand(deck.Skip(54).Take(27)),
            [PlayerSeat.West] = new PlayerHand(deck.Skip(81).Take(27)),
        };
    }

    private void AdvanceAfterPlay(GameState gameState, PlayerSeat seat)
    {
        SettlementResult? settlement = settlementRule.TryBuild(gameState);

        if (settlement is not null)
        {
            gameState.CompleteGame(settlement);
            return;
        }

        PlayerSeat nextSeat = roundFlowRule.GetNextSeat(gameState, seat);
        gameState.MoveTurnTo(nextSeat);
    }

    private void AdvanceAfterPass(GameState gameState, PlayerSeat seat)
    {
        if (roundFlowRule.ShouldResetRound(gameState))
        {
            PlayerSeat leadSeat = gameState.Round.LeadSeat ?? seat;
            gameState.ResetRound();
            gameState.MoveTurnTo(leadSeat);
            return;
        }

        PlayerSeat nextSeat = roundFlowRule.GetNextSeat(gameState, seat);
        gameState.MoveTurnTo(nextSeat);
    }

    private GameView ToGameView(GameState gameState)
    {
        GameRankContext context = new(gameState.LevelRank);
        PlayedComboView? leadingPlay = ToLeadingPlay(gameState, context);
        SettlementView? settlement = ToSettlement(gameState);

        return new GameView(
            gameState.Status.ToString(),
            gameState.LevelRank.ToString(),
            (int)gameState.CurrentTurn,
            gameState.Round.IsFreshRound,
            gameState.Round.ConsecutivePasses,
            ToPlayers(gameState, context),
            leadingPlay,
            gameState.ActionLogs.ToList(),
            settlement);
    }

    private IReadOnlyList<PlayerView> ToPlayers(GameState gameState, GameRankContext context)
    {
        return Enum.GetValues<PlayerSeat>()
            .Select(seat =>
            {
                IReadOnlyList<Card> sortedCards = cardOrderingService.SortCards(gameState.GetHand(seat).Cards, context);
                IReadOnlyList<CardView> cards = sortedCards.Select(card => new CardView(card.Id, card.ToDisplayText())).ToList();
                return new PlayerView((int)seat, cards.Count, gameState.CurrentTurn == seat, gameState.IsFinished(seat), cards);
            })
            .ToList();
    }

    private PlayedComboView? ToLeadingPlay(GameState gameState, GameRankContext context)
    {
        if (gameState.Round.LeadingPlay is null)
        {
            return null;
        }

        IReadOnlyList<Card> cards = cardOrderingService.SortCards(gameState.Round.LeadingPlay.Cards, context);
        return new PlayedComboView((int)gameState.Round.LeadingPlay.Seat, cards.Select(card => new CardView(card.Id, card.ToDisplayText())).ToList());
    }

    private SettlementView? ToSettlement(GameState gameState)
    {
        if (gameState.Settlement is null)
        {
            return null;
        }

        IReadOnlyList<string> finishOrder = gameState.Settlement.FinishOrder
            .Select(item => $"{(int)item.Seat}号位第{item.Rank}名")
            .ToList();

        return new SettlementView(
            gameState.Settlement.WinningTeam.Select(seat => (int)seat).ToList(),
            gameState.Settlement.IsDoubleDown,
            finishOrder);
    }
}
