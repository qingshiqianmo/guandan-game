using Guandan.Game.Rules.Patterns;

namespace Guandan.Game.Rules.Validation;

public sealed class PlayComparisonService
{
    public bool CanBeat(PatternDescriptor candidate, PatternDescriptor target)
    {
        if (target.Type == PatternType.JokerBomb)
        {
            return false;
        }

        if (candidate.Type == PatternType.JokerBomb)
        {
            return true;
        }

        if (candidate.Type == PatternType.StraightFlush)
        {
            return target.Type != PatternType.JokerBomb && target.Type != PatternType.StraightFlush
                || CompareSameType(candidate, target);
        }

        if (candidate.Type == PatternType.Bomb)
        {
            return target.Type != PatternType.Bomb && target.Type != PatternType.StraightFlush
                ? true
                : CompareBomb(candidate, target);
        }

        if (candidate.Type != target.Type)
        {
            return false;
        }

        return CompareSameType(candidate, target);
    }

    private bool CompareSameType(PatternDescriptor candidate, PatternDescriptor target)
    {
        if (candidate.Type == PatternType.Bomb)
        {
            return CompareBomb(candidate, target);
        }

        if (candidate.Type == PatternType.StraightFlush)
        {
            return CompareSequence(candidate, target);
        }

        if (candidate.CardCount != target.CardCount)
        {
            return false;
        }

        return candidate.PrimaryStrength > target.PrimaryStrength;
    }

    private bool CompareBomb(PatternDescriptor candidate, PatternDescriptor target)
    {
        if (target.Type == PatternType.StraightFlush)
        {
            return false;
        }

        if (candidate.CardCount != target.CardCount)
        {
            return candidate.CardCount > target.CardCount;
        }

        return candidate.PrimaryStrength > target.PrimaryStrength;
    }

    private bool CompareSequence(PatternDescriptor candidate, PatternDescriptor target)
    {
        if (candidate.CardCount != target.CardCount)
        {
            return false;
        }

        return candidate.PrimaryStrength > target.PrimaryStrength;
    }
}
