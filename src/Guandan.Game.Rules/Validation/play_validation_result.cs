using Guandan.Game.Domain.Cards;
using Guandan.Game.Rules.Patterns;

namespace Guandan.Game.Rules.Validation;

public sealed record PlayValidationResult(
    bool IsValid,
    string? ErrorCode,
    PatternDescriptor? Pattern,
    IReadOnlyList<Card>? SortedCards)
{
    public static PlayValidationResult Success(PatternDescriptor pattern, IReadOnlyList<Card> sortedCards)
    {
        return new PlayValidationResult(true, null, pattern, sortedCards);
    }

    public static PlayValidationResult Failure(string errorCode)
    {
        return new PlayValidationResult(false, errorCode, null, null);
    }
}
