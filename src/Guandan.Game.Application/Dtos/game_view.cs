namespace Guandan.Game.Application.Dtos;

public sealed record GameView(
    string Status,
    string LevelRank,
    int CurrentTurn,
    bool IsFreshRound,
    int ConsecutivePasses,
    IReadOnlyList<PlayerView> Players,
    PlayedComboView? LeadingPlay,
    IReadOnlyList<string> Logs,
    SettlementView? Settlement);
