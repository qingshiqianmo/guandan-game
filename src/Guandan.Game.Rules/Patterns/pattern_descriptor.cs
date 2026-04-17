namespace Guandan.Game.Rules.Patterns;

public sealed record PatternDescriptor(
    PatternType Type,
    int PrimaryStrength,
    int SecondaryStrength,
    int CardCount,
    int SequenceLength,
    int Priority);
