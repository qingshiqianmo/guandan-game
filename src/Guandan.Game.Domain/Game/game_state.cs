using Guandan.Game.Domain.Cards;
using Guandan.Game.Domain.Players;

namespace Guandan.Game.Domain.Game;

public sealed class GameState
{
    private readonly Dictionary<PlayerSeat, PlayerHand> hands;
    private readonly List<PlayerFinishRecord> finishOrder;
    private readonly List<string> actionLogs;

    private GameState(
        CardRank levelRank,
        Dictionary<PlayerSeat, PlayerHand> hands,
        PlayerSeat currentTurn)
    {
        LevelRank = levelRank;
        this.hands = hands;
        CurrentTurn = currentTurn;
        Status = GameStatus.InProgress;
        Round = new RoundState(null, null, 0);
        finishOrder = [];
        actionLogs = ["牌局已开始"];
    }

    public CardRank LevelRank { get; }

    public GameStatus Status { get; private set; }

    public PlayerSeat CurrentTurn { get; private set; }

    public RoundState Round { get; }

    public SettlementResult? Settlement { get; private set; }

    public IReadOnlyDictionary<PlayerSeat, PlayerHand> Hands => hands;

    public IReadOnlyList<PlayerFinishRecord> FinishOrder => finishOrder;

    public IReadOnlyList<string> ActionLogs => actionLogs;

    public static GameState Create(CardRank levelRank, Dictionary<PlayerSeat, PlayerHand> hands)
    {
        return new GameState(levelRank, hands, PlayerSeat.North);
    }

    public PlayerHand GetHand(PlayerSeat seat)
    {
        return hands[seat];
    }

    public IReadOnlyList<PlayerSeat> GetActiveSeats()
    {
        return Enum.GetValues<PlayerSeat>().Where(seat => !IsFinished(seat)).ToList();
    }

    public bool IsFinished(PlayerSeat seat)
    {
        return finishOrder.Any(record => record.Seat == seat);
    }

    public void ApplySuccessfulPlay(PlayerSeat seat, IReadOnlyList<Card> cards)
    {
        hands[seat].RemoveByIds(cards.Select(card => card.Id));
        Round.SetLeadingPlay(new PlayedCombo(seat, cards));
        AppendLog($"{(int)seat}号位出牌：{string.Join(' ', cards.Select(card => card.ToDisplayText()))}");
        RecordFinishIfNeeded(seat);
    }

    public void ApplyPass(PlayerSeat seat)
    {
        Round.AddPass();
        AppendLog($"{(int)seat}号位过牌");
    }

    public void ResetRound()
    {
        Round.ResetForNewRound();
        AppendLog("新一轮开始");
    }

    public void MoveTurnTo(PlayerSeat seat)
    {
        CurrentTurn = seat;
    }

    public void CompleteGame(SettlementResult settlementResult)
    {
        Status = GameStatus.Finished;
        Settlement = settlementResult;
        AppendLog("牌局结束");
    }

    public void AppendLog(string message)
    {
        actionLogs.Add(message);
    }

    private void RecordFinishIfNeeded(PlayerSeat seat)
    {
        if (hands[seat].Count > 0 || IsFinished(seat))
        {
            return;
        }

        finishOrder.Add(new PlayerFinishRecord(seat, finishOrder.Count + 1));
        AppendLog($"{(int)seat}号位出完手牌，获得第{finishOrder.Count}名");
    }
}
