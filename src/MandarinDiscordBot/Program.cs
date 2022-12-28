using Discord;
using Discord.WebSocket;
using MandarinDiscordBot.Constants;

public class Program
{
    public static Task Main(string[] args) => new Program().MainAsync();

    private DiscordSocketClient _client;

    public async Task MainAsync()
    {
        _client = new DiscordSocketClient();

        //_client.Log += Log;

        var token = Environment.GetEnvironmentVariable(EnvironmentConstants.KeyName);

        if(token == null) 
        {
            return;
        }

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        await Task.Delay(-1);
    }
}