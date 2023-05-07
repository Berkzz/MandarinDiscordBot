using MandarinDiscordBot.Services.Audio;
using MandarinDiscordBot.Services.Music;

namespace MandarinDiscordBot.Services.Extensions;

public static class SongExtensions
{
    public static TimeSpan GetTotalTimeSpan(this IEnumerable<Song> songs)
    {
        return songs.Select(x => x.VideoInfo).GetTotalTimeSpan();
    }

    public static TimeSpan GetTotalTimeSpan(this IEnumerable<VideoInfo> videoInfos)
    {
        return TimeSpan.FromSeconds(videoInfos.Sum(x => x.Duration.Value.TotalSeconds));
    }
}