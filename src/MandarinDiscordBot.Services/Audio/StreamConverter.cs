using CliWrap;

namespace MandarinDiscordBot.Services.Audio;

public class StreamConverter
{
    public async Task<MemoryStream> GetMemoryStream(Stream stream)
    {
        var memoryStream = new MemoryStream();

        await Cli.Wrap("ffmpeg")
            .WithArguments(" -hide_banner -loglevel panic -i pipe:0 -ac 2 -f s16le -ar 48000 pipe:1")
            .WithStandardInputPipe(PipeSource.FromStream(stream))
            .WithStandardOutputPipe(PipeTarget.ToStream(memoryStream))
            .ExecuteAsync();

        return memoryStream;
    }
}