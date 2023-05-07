using MandarinDiscordBot.Services.Enums;
using MandarinDiscordBot.Services.Music;
using YoutubeExplode;
using YoutubeExplode.Common;
using YoutubeExplode.Playlists;
using YoutubeExplode.Search;
using YoutubeExplode.Videos;
using YoutubeExplode.Videos.Streams;

namespace MandarinDiscordBot.Services.Audio;

public class YoutubeService
{
    private readonly YoutubeClient _youtube;

    public YoutubeService()
    {
        _youtube = new YoutubeClient();
    }

    public async Task<Stream> GetAudioStream(string videoUrl)
    {
        var streamManifest = await _youtube.Videos.Streams.GetManifestAsync(videoUrl);

        var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

        return await _youtube.Videos.Streams.GetAsync(streamInfo);
    }

    public SongRequestType GetSongRequestType(string searchRequest)
    {
        if (searchRequest.Contains("&list="))
        {
            return SongRequestType.Playlist;
        }

        if (!searchRequest.Contains("https://") && !searchRequest.Contains("http://"))
        {
            return SongRequestType.Url;
        }

        return SongRequestType.Search;
    }

    public async Task<IEnumerable<Song>> GetSongsFromRequest(string searchRequest)
    {
        var infos = await GetVideoInfos(searchRequest);

        var result = new List<Song>(infos.Count());

        foreach (var info in infos)
        {
            result.Add(new Song(info, await GetAudioStream(info.Url)));
        }

        return result;
    }

    public async Task<IEnumerable<VideoInfo>> GetVideoInfos(string searchRequest)
    {
        var songRequestType = GetSongRequestType(searchRequest);

        if (songRequestType == SongRequestType.Playlist)
        {
            return (await GetPlaylistInfo(searchRequest)).Select(x => new VideoInfo(x.Title, x.Duration, x.Url));
        }

        if (songRequestType == SongRequestType.Url)
        {
            var info = await GetVideoInfo(searchRequest);

            return new List<VideoInfo> { new VideoInfo(info.Title, info.Duration, info.Url) };
        }

        if (songRequestType == SongRequestType.Search)
        {
            var info = await GetSearchInfo(searchRequest);

            return new List<VideoInfo> { new VideoInfo(info.Title, info.Duration, info.Url) };
        }

        return new List<VideoInfo>();
    }

    private async Task<VideoSearchResult> GetSearchInfo(string searchQuery)
    {
        return await _youtube.Search.GetVideosAsync(searchQuery).FirstAsync();
    }

    private async Task<IEnumerable<PlaylistVideo>> GetPlaylistInfo(string playlistUrl)
    {
        return await _youtube.Playlists.GetVideosAsync(playlistUrl);
    }

    private async Task<Video> GetVideoInfo(string videoUrl)
    {
        return await _youtube.Videos.GetAsync(videoUrl);
    }
}
