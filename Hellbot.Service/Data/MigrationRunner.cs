using FluentMigrator.Runner;

namespace Hellbot.Service.Data
{
    public class MigrationRunner(IServiceProvider provider): IHostedService
    {

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = provider.CreateScope();

            var runner = scope.ServiceProvider
                .GetRequiredService<IMigrationRunner>();

            runner.MigrateUp();
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
