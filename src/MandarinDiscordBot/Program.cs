using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using MandarinDiscordBot.Constants;
using MandarinDiscordBot.Extensions;

public class Program
{
    public static Task Main() => new Program().MainAsync();

    private DiscordSocketClient _client;
    private InteractionService _interactionService;

    private readonly IServiceProvider _serviceProvider;

    public Program()
    {
        _client = new DiscordSocketClient();
        _serviceProvider = BotServices.BuildServiceProvider();
        _interactionService = new InteractionService(_client.Rest);
    }

    public async Task MainAsync()
    {
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

        //_client.Log += Log;
        _client.Ready += Client_Ready;

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }

    private async Task Client_Ready()
    {
        await _interactionService.AddModulesAsync(GetType().Assembly, _serviceProvider);

    #if DEBUG

        if (ulong.TryParse(Environment.GetEnvironmentVariable(EnvironmentConstants.TestGuildIdName), out var guildId))
        {
            await _interactionService.RegisterCommandsToGuildAsync(guildId);
        }

    #else

        await interactionService.RegisterCommandsGloballyAsync();

    #endif

    }
}