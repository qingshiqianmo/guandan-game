using Guandan.Game.Application.Abstractions;
using Guandan.Game.Application.Services;
using Guandan.Game.Rules.Patterns;
using Guandan.Game.Rules.Ranking;
using Guandan.Game.Rules.Rounds;
using Guandan.Game.Rules.Settlement;
using Guandan.Game.Rules.Validation;
using Guandan.Game.Rules.Wildcards;
using Guandan.Game.Web.Endpoints;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGameSessionStore, InMemoryGameSessionStore>();
builder.Services.AddSingleton<IShuffleService, DefaultShuffleService>();
builder.Services.AddSingleton<CardOrderingService>();
builder.Services.AddSingleton<WildcardExpansionService>();
builder.Services.AddSingleton<PatternRecognizer>();
builder.Services.AddSingleton<PlayComparisonService>();
builder.Services.AddSingleton<PlayValidationService>();
builder.Services.AddSingleton<RoundFlowRule>();
builder.Services.AddSingleton<SettlementRule>();
builder.Services.AddSingleton<GameApplicationService>();

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();
app.MapGameEndpoints();

// 指定明确的IP地址和端口9009
app.Urls.Add("http://127.0.0.1:9009");

app.Run();
