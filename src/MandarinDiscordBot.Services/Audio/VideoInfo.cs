namespace MandarinDiscordBot.Services.Audio;

public record VideoInfo
{
    public VideoInfo(string title, TimeSpan? duration, string url)
    {
        Title = title;
        Duration = duration;
        Url = url;
    }

    public string Title { get; }
    public TimeSpan? Duration { get; }
    public string Url { get; }
}