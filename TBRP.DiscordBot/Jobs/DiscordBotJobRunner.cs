using Microsoft.Extensions.DependencyInjection;

namespace TBRP.DiscordBot.Jobs;

public sealed class DiscordBotJobRunner(IServiceProvider serviceProvider)
{
    private readonly List<Task> _runningJobs = [];

    public void StartAll(CancellationToken cancellationToken)
    {
        var jobs = serviceProvider.GetServices<IDiscordBotJob>().ToArray();

        if (jobs.Length == 0)
        {
            DiscordBotClient.Log("No Discord bot jobs registered.");
            return;
        }

        foreach (var job in jobs)
        {
            DiscordBotClient.Log($"Starting Discord bot job: {job.Name}");

            _runningJobs.Add(Task.Run(async () =>
            {
                try
                {
                    await job.ExecuteAsync(cancellationToken);
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    DiscordBotClient.Log($"Discord bot job stopped: {job.Name}");
                }
                catch (Exception exception)
                {
                    DiscordBotClient.Log($"Discord bot job failed: {job.Name}");
                    Console.WriteLine(exception);
                }
            }, cancellationToken));
        }
    }
}
