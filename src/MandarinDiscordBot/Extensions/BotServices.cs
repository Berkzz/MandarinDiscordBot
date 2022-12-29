using Microsoft.Extensions.DependencyInjection;

namespace MandarinDiscordBot.Extensions;

public static class BotServices
{
    public static IServiceProvider? Provider { get; private set; }

    public static IServiceProvider BuildServiceProvider()
    {
        Provider = ConfigureServices().BuildServiceProvider();

        return Provider;
    }

    private static IServiceCollection ConfigureServices()
    {
        var serviceCollection = new ServiceCollection();

        return serviceCollection;
    }
}