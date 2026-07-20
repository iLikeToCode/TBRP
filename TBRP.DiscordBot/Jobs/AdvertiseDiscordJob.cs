using TBRP.Api;

namespace TBRP.DiscordBot.Jobs;

public sealed class AdvertiseDiscordJob (ApiClient apiClient) : PeriodicDiscordBotJob
{
    protected override TimeSpan Interval => TimeSpan.FromMinutes(3);

    protected override async Task ExecuteOnceAsync(CancellationToken cancellationToken)
    {
        await apiClient.ErlcV2.SendCommand(":h Join our server today - code: BrwFrxmSN");
        return;
    }
}
