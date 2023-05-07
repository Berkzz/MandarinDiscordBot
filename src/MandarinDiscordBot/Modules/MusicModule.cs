using AngleSharp.Io;
using AngleSharp.Media;
using Discord;
using Discord.Interactions;
using MandarinDiscordBot.Services.Audio;
using MandarinDiscordBot.Services.Enums;
using MandarinDiscordBot.Services.Extensions;
using MandarinDiscordBot.Services.Music;

namespace MandarinDiscordBot.Modules;

public class MusicModule : InteractionModuleBase
{
    private YoutubeService _youtube;
    private GuildPool _guilds;

    public MusicModule(YoutubeService youtube, GuildPool guildPool)
    {
        _youtube = youtube;
        _guilds = guildPool;
    }

    #region RepetativeCode

    private bool TryGetVoiceChannel(IVoiceChannel? channel, out IVoiceChannel? outVoiceChannel)
    {
        if(channel != null)
        {
            outVoiceChannel = channel;

            return true;
        }

        var actualChannel = (Context.User as IGuildUser)?.VoiceChannel;

        if(actualChannel != null)
        {
            outVoiceChannel = actualChannel;

            return true;
        }

        outVoiceChannel = null;

        return false;
    }

    private async Task<bool> BotInVoiceChannel(IVoiceChannel channel)
    {
        return await channel.GetUserAsync(Context.Client.CurrentUser.Id) != null;
    }

    #endregion

    [SlashCommand("connect", "Connects bot to voice channel", runMode: RunMode.Async)]
    public async Task JoinVoiceChannel(
        [Summary(description: "Voice channel bot connects to")] IVoiceChannel? channel = null)
    {
        if (!TryGetVoiceChannel(channel, out var actualVoiceChannel))
        {
            await RespondAsync("You must be in the channel, or pass voice channel into parameter", ephemeral: true);

            return;
        }

        var voiceChannelBot = await actualVoiceChannel!.GetUserAsync(Context.Client.CurrentUser.Id);

        if (voiceChannelBot != null)
        {
            await RespondAsync("Bot is already in this voice channel", ephemeral: true);

            return;
        }

        var audioClient = await actualVoiceChannel.ConnectAsync(true);

        _guilds.GetOrAddGuild(Context.Guild).Queue.AttachAudioClient(audioClient);

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

    [SlashCommand("play", "Plays music from Youtube", runMode: RunMode.Async)]
    public async Task PlayYoutubeMusic(
        [Summary(description: "Youtube video url / search request / playlist")] string request, 
        [Summary(description: "Voice channel bot plays music in")] IVoiceChannel? channel = null)
    {
        if (!TryGetVoiceChannel(channel, out var actualVoiceChannel))
        {
            await RespondAsync("You must be in the channel, or pass voice channel into parameter", ephemeral: true);

            return;
        }

        var guild = _guilds.GetOrAddGuild(Context.Guild);

        if (!await BotInVoiceChannel(actualVoiceChannel!))
        {
            var audioClient = await actualVoiceChannel!.ConnectAsync(true);

            guild.Queue.AttachAudioClient(audioClient);
        }

        var searchRequestType = _youtube.GetSongRequestType(request);

        var videoInfos = await _youtube.GetVideoInfos(request);

        var response = "Added songs:\n" +
                       string.Concat(videoInfos.Select(x => $"> {x.Title}\n")) +
                       $"Total duration: {videoInfos.GetTotalTimeSpan()}";

        await RespondAsync(response, ephemeral: true);

        await guild.Queue.AddSong(request);
    }

    [SlashCommand("skip", "Skips current song", runMode: RunMode.Async)]
    public async Task SkipSong()
    {
        if(!_guilds.TryGetGuild(Context.Guild, out var guild))
        {
            await RespondAsync("Nothing to skip", ephemeral: true);
        }

        guild.SkipSong();

        await RespondAsync("Skipped", ephemeral: true);
    }

    [SlashCommand("queue", "Shows queue", runMode: RunMode.Async)]
    public async Task ShowQueue()
    {
        if (!_guilds.TryGetGuild(Context.Guild, out var guild))
        {
            await RespondAsync("No queue", ephemeral: true);

            return;
        }

        await RespondAsync($"Current song: {guild.Queue.CurrentSong.VideoInfo.Title} \n\n" +
                           $"Queue: \n{string.Concat(guild.Queue.GetSongList().Select(x => $"> {x.VideoInfo.Title}\n"))}" +
                           $"Total duration: {guild.Queue.GetSongList().GetTotalTimeSpan()}");
    }

}
