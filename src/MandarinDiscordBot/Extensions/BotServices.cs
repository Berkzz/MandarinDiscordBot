using MandarinDiscordBot.Services.Audio;
using MandarinDiscordBot.Services.Music;
using Microsoft.Extensions.DependencyInjection;

namespace MandarinDiscordBot.Extensions;

public static class BotServices
{
    private static IServiceProvider? Provider { get; set; }

    public static IServiceProvider BuildServiceProvider()
    {
        Provider = ConfigureServices().BuildServiceProvider();

        return Provider;
    }

    private static IServiceCollection ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        serviceCollection.AddSingleton<YoutubeService>();
        serviceCollection.AddSingleton<GuildPool>();
        serviceCollection.AddSingleton<StreamConverter>();

        return serviceCollection;
    }
}