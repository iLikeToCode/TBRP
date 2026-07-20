namespace TBRP.DiscordBot.Jobs;

public abstract class PeriodicDiscordBotJob : IDiscordBotJob
{
    public virtual string Name => GetType().Name;

    protected abstract TimeSpan Interval { get; }

    protected virtual bool RunImmediately => true;

    public async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        if (RunImmediately)
            await ExecuteOnceSafelyAsync(cancellationToken);

        using var timer = new PeriodicTimer(Interval);

        while (await timer.WaitForNextTickAsync(cancellationToken))
        {
            await ExecuteOnceSafelyAsync(cancellationToken);
        }
    }

    protected abstract Task ExecuteOnceAsync(CancellationToken cancellationToken);

    private async Task ExecuteOnceSafelyAsync(CancellationToken cancellationToken)
    {
        try
        {
            await ExecuteOnceAsync(cancellationToken);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            DiscordBotClient.Log($"Discord bot job run failed: {Name}");
            Console.WriteLine(exception);
        }
    }
}
