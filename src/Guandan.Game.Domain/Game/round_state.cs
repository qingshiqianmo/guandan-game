using Guandan.Game.Domain.Players;

namespace Guandan.Game.Domain.Game;

public sealed class RoundState
{
    public RoundState(PlayedCombo? leadingPlay, PlayerSeat? leadSeat, int consecutivePasses)
    {
        LeadingPlay = leadingPlay;
        LeadSeat = leadSeat;
        ConsecutivePasses = consecutivePasses;
    }

    public PlayedCombo? LeadingPlay { get; private set; }

    public PlayerSeat? LeadSeat { get; private set; }

    public int ConsecutivePasses { get; private set; }

    public bool IsFreshRound => LeadingPlay is null;

    public void SetLeadingPlay(PlayedCombo combo)
    {
        LeadingPlay = combo;
        LeadSeat = combo.Seat;
        ConsecutivePasses = 0;
    }

    public void AddPass()
    {
        ConsecutivePasses++;
    }

    public void ResetForNewRound()
    {
        LeadingPlay = null;
        LeadSeat = null;
        ConsecutivePasses = 0;
    }
}
