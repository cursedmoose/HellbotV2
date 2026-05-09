using Hellbot.Service.Config;
using Hellbot.Service.Data.Tables;
using Microsoft.Extensions.Options;

namespace Hellbot.Service.Stats;

public sealed class UserStatsFlushWorker(
    UserStatsRecorder recorder,
    IServiceScopeFactory scopeFactory,
    IOptionsMonitor<UserStatsOptions> options,
    ILogger<UserStatsFlushWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var flushInterval = options.CurrentValue.FlushInterval;
                if (flushInterval < TimeSpan.FromSeconds(10))
                    flushInterval = TimeSpan.FromSeconds(10);

                try
                {
                    await Task.Delay(flushInterval, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }

                await TryFlushOnceAsync(stoppingToken);
            }
        }
        finally
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
            try
            {
                await TryFlushOnceAsync(cts.Token);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "User stats flush on shutdown failed");
            }
        }
    }

    private async Task TryFlushOnceAsync(CancellationToken ct)
    {
        try
        {
            using var scope = scopeFactory.CreateScope();
            var table = scope.ServiceProvider.GetRequiredService<UserStatTable>();
            await recorder.FlushPendingAsync(table, ct);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "User stats flush failed");
        }
    }
}
