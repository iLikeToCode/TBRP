namespace TBRP.DiscordBot.Jobs;

public abstract class PeriodicDiscordBotJob : IDiscordBotJob
{
    public virtual string Name => GetType().Name;

    protected abstract TimeSpan Interval { get; }

    protected virtual bool RunImmediately => true;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (RunImmediately)
            await ExecuteOnceAsync(cancellationToken);

        using var timer = new PeriodicTimer(Interval);

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            await ExecuteOnceAsync(cancellationToken);
        }
    }

    protected abstract Task ExecuteOnceAsync(CancellationToken cancellationToken);
}
