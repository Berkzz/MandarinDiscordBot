using Discord;
using Discord.Interactions;
using Discord.Net;
using Discord.WebSocket;
using MandarinDiscordBot.Commands;
using MandarinDiscordBot.Constants;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;

public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();

    private DiscordSocketClient _client;
    private InteractionService _interactionService;

    private readonly IServiceProvider _serviceProvider;

    public Program()
    {
        _serviceProvider = CreateProvider();
    }

    static IServiceProvider CreateProvider()
    {
        var collection = new ServiceCollection();

        return collection.BuildServiceProvider();
    }

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();

        //_client.Log += Log;
        _client.Ready += Client_Ready;

        _interactionService = new InteractionService(_client.Rest);

        var token = Environment.GetEnvironmentVariable(EnvironmentConstants.KeyName);

        if (token == null)
        {
            return;
        }

        _client.InteractionCreated += async (x) =>
        {
            var ctx = new SocketInteractionContext(_client, x);
            await _interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
        };

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private async Task Client_Ready()
    {
        await _interactionService.AddModulesAsync(GetType().Assembly, _serviceProvider);
        await _interactionService.RegisterCommandsGloballyAsync();
    }
}