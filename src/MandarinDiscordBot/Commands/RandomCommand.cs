﻿using Discord.Interactions;
using Pcg;

namespace MandarinDiscordBot.Commands;

public class RandomCommand : InteractionModuleBase
{
    private readonly PcgRandom _random;

    public RandomCommand()
    {
        _random = new PcgRandom();
    }

    [SlashCommand("random", "Gives random number", runMode: RunMode.Async)]
    public async Task Blep([Summary(description: "Min value included")] int minValue = int.MinValue, [Summary(description: "Max value included")] [MaxValue(int.MaxValue - 1)] int maxValue = int.MaxValue - 1)
    {
        if(maxValue < minValue)
        {
            await RespondAsync("Max value must be greater than min value", ephemeral: true);
        }

        await RespondAsync(_random.Next(minValue, maxValue + 1).ToString());        
    }
}