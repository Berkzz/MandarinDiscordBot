using Discord;
using Discord.Interactions;

namespace MandarinDiscordBot.Modules;

public class MusicModule : InteractionModuleBase
{
    public MusicModule()
    {

    }

    [SlashCommand("connect", "Connects bot to voice channel", runMode: RunMode.Async)]
    public async Task JoinVoiceChannel([Summary(description: "Voice channel bot connects to")] IVoiceChannel? channel = null)
    {
        channel ??= (Context.User as IGuildUser)?.VoiceChannel;

        if (channel == null)
        {
            await RespondAsync("You must be in the channel, or pass voice channel into parameter", ephemeral: true);

            return;
        }

        var voiceChannelBot = await channel.GetUserAsync(Context.Client.CurrentUser.Id);

        if (voiceChannelBot != null)
        {
            await RespondAsync("Bot is already in this voice channel", ephemeral: true);

            return;
        }

        await channel.ConnectAsync(true, external: true);

        await RespondAsync("Connected successfully", ephemeral: true);
    }

    [SlashCommand("disconnect", "Disconnects bot from voice channel and stops music", runMode: RunMode.Async)]
    public async Task LeaveVoiceChannel()
    {
        var voiceChannel = (await Context.Guild.GetCurrentUserAsync())?.VoiceChannel;

        if(voiceChannel == null)
        {
            await RespondAsync("Bot is not in channel", ephemeral: true);

            return;
        }

        await voiceChannel.DisconnectAsync();

        await RespondAsync("Disconnected successfully", ephemeral: true);
    }
}
