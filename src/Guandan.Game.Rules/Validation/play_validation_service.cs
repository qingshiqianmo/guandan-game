using Guandan.Game.Domain.Game;
using Guandan.Game.Domain.Players;
using Guandan.Game.Rules.Patterns;
using Guandan.Game.Rules.Ranking;

namespace Guandan.Game.Rules.Validation;

public sealed class PlayValidationService
{
    private readonly CardOrderingService cardOrderingService;
    private readonly PatternRecognizer patternRecognizer;
    private readonly PlayComparisonService playComparisonService;

    public PlayValidationService(
        CardOrderingService cardOrderingService,
        PatternRecognizer patternRecognizer,
        PlayComparisonService playComparisonService)
    {
        this.cardOrderingService = cardOrderingService;
        this.patternRecognizer = patternRecognizer;
        this.playComparisonService = playComparisonService;
    }

    public PlayValidationResult ValidatePlay(
        GameState gameState,
        PlayerSeat seat,
        IReadOnlyList<string> cardIds)
    {
        if (gameState.Status == GameStatus.Finished)
        {
            return PlayValidationResult.Failure("game_already_finished");
        }

        if (gameState.CurrentTurn != seat)
        {
            return PlayValidationResult.Failure("not_player_turn");
        }

        if (!gameState.GetHand(seat).ContainsAll(cardIds))
        {
            return PlayValidationResult.Failure("cards_not_in_hand");
        }

        GameRankContext context = new(gameState.LevelRank);
        IReadOnlyList<Domain.Cards.Card> selectedCards = gameState.GetHand(seat).FindByIds(cardIds);
        PatternDescriptor? pattern = patternRecognizer.Recognize(selectedCards, context);

        if (pattern is null)
        {
            return PlayValidationResult.Failure("invalid_pattern");
        }

        IReadOnlyList<Domain.Cards.Card> sortedCards = cardOrderingService.SortCards(selectedCards, context);

        if (gameState.Round.IsFreshRound)
        {
            return PlayValidationResult.Success(pattern, sortedCards);
        }

        PatternDescriptor? leadPattern = patternRecognizer.Recognize(gameState.Round.LeadingPlay!.Cards, context);

        if (leadPattern is null || !playComparisonService.CanBeat(pattern, leadPattern))
        {
            return PlayValidationResult.Failure("play_not_big_enough");
        }

        return PlayValidationResult.Success(pattern, sortedCards);
    }

    public string? ValidatePass(GameState gameState, PlayerSeat seat)
    {
        if (gameState.Status == GameStatus.Finished)
        {
            return "game_already_finished";
        }

        if (gameState.CurrentTurn != seat)
        {
            return "not_player_turn";
        }

        return gameState.Round.IsFreshRound ? "cannot_pass_on_fresh_round" : null;
    }
}
