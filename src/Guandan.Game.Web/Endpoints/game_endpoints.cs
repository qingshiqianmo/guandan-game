using Guandan.Game.Application.Commands;
using Guandan.Game.Application.Services;

namespace Guandan.Game.Web.Endpoints;

public static class GameEndpoints
{
    public static IEndpointRouteBuilder MapGameEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/game/start", (GameApplicationService service) =>
        {
            return Results.Ok(service.StartGame(new StartGameCommand()));
        });

        endpoints.MapGet("/api/game/state", (GameApplicationService service) =>
        {
            return service.GetGameState() is { } game ? Results.Ok(game) : Results.NotFound();
        });

        endpoints.MapPost("/api/game/play", (PlayCardsCommand command, GameApplicationService service) =>
        {
            return Results.Ok(service.PlayCards(command));
        });

        endpoints.MapPost("/api/game/pass", (PassTurnCommand command, GameApplicationService service) =>
        {
            return Results.Ok(service.PassTurn(command));
        });

        endpoints.MapPost("/api/game/restart", (GameApplicationService service) =>
        {
            return Results.Ok(service.RestartGame());
        });

        return endpoints;
    }
}
