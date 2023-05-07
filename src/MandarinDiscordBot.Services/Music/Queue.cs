using CliWrap;
using Discord.Audio;
using MandarinDiscordBot.Services.Audio;
using YoutubeExplode.Videos;

namespace MandarinDiscordBot.Services.Music;

public class Queue
{
    private readonly YoutubeService _youtube;
    private readonly StreamConverter _streamConverter;

    private Queue<Song> _songList = new Queue<Song>(8);
    private IAudioClient _audioClient;
    private CancellationTokenSource? _tokenSource;
    private AudioOutStream _pcmStream;

    public Song? CurrentSong { get; private set; }

    public bool IsPlaying { get; private set; }

    public Queue()
    {
        _youtube = new YoutubeService();
        _streamConverter = new StreamConverter();
    }

    public IEnumerable<Song> GetSongList()
    {
        return _songList.ToList();
    }

    public void AttachAudioClient(IAudioClient audioClient)
    {
        _audioClient?.Dispose();

        _audioClient = audioClient;

        _pcmStream = _audioClient.CreatePCMStream(AudioApplication.Music, bitrate: 96000);
    }

    private void AddSongList(IEnumerable<Song> songs)
    {
        foreach (var song in songs)
        {
            _songList.Enqueue(song);
        }
    }

    public async Task AddSong(string searchRequest)
    {
        var songs = await _youtube.GetSongsFromRequest(searchRequest);

        AddSongList(songs);

        if(!IsPlaying)
        {
            await SongLoop();
        }
    }

    public void SkipSong()
    {
        if(_tokenSource == null || _tokenSource.IsCancellationRequested)
        {
            return;
        }

        _tokenSource?.Cancel();
    }

    private async Task SongLoop()
    {
        while (true)
        {
            _tokenSource = new CancellationTokenSource();

            CurrentSong = _songList.Dequeue();

            var memoryStream = await _streamConverter.GetMemoryStream(CurrentSong.Stream);

            IsPlaying = true;

            try
            {
                _tokenSource = new CancellationTokenSource();
                await _pcmStream.WriteAsync(memoryStream.ToArray(), _tokenSource.Token);
            }
            catch (Exception)
            {
                Console.WriteLine("Song cancelled!");
            }

            if (!_songList.Any())
            {
                break;
            }
        }

        IsPlaying = false;
        CurrentSong = null;
    }
}