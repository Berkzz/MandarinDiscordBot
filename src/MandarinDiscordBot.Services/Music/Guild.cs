namespace MandarinDiscordBot.Services.Music;

public class Guild
{
    public ulong Id { get; }
    public Queue Queue { get; } = new Queue();

    public Guild(ulong id)
    {
        Id = id;
    }

    public void SkipSong()
    {
        Queue.SkipSong();
    }
}