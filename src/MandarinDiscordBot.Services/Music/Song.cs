using MandarinDiscordBot.Services.Audio;

namespace MandarinDiscordBot.Services.Music;

public class Song
{
    public Song(VideoInfo videoInfo, Stream stream)
    {
        VideoInfo = videoInfo;
        Stream = stream;
    }

    public VideoInfo VideoInfo { get; }
    public Stream Stream { get; }
}
