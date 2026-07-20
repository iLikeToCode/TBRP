namespace TBRP.DiscordBot.Jobs;

public interface IDiscordBotJob
{
    string Name => GetType().Name;

    Task ExecuteAsync(CancellationToken cancellationToken);
}
