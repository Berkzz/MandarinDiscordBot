using Discord;

namespace MandarinDiscordBot.Services.Music;

public class GuildPool
{
    private List<Guild> _guilds = new List<Guild>();

    public Guild GetOrAddGuild(IGuild guild)
    {
        var existingGuild = _guilds.SingleOrDefault(x => x.Id == guild.Id);

        if(existingGuild == null)
        {
            var newGuild = new Guild(guild.Id);

            _guilds.Add(newGuild);

            return newGuild;
        }

        return existingGuild;
    }

    public bool TryGetGuild(IGuild guild, out Guild? outGuild)
    {
        var existingGuild = _guilds.SingleOrDefault(x => x.Id == guild.Id);

        if(existingGuild == null)
        {
            outGuild = null;

            return false;
        }

        outGuild = existingGuild;

        return true;
    }
}
