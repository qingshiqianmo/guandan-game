namespace Guandan.Game.Application.Dtos;

public sealed record ActionResult(bool IsSuccess, string? ErrorCode, GameView? Game)
{
    public static ActionResult Success(GameView game)
    {
        return new ActionResult(true, null, game);
    }

    public static ActionResult Failure(string errorCode, GameView? game)
    {
        return new ActionResult(false, errorCode, game);
    }
}
