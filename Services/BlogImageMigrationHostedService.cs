namespace WebThuMuaPheLieu.Services;

public sealed class BlogImageMigrationHostedService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<BlogImageMigrationHostedService> _logger;
    private readonly IConfiguration _configuration;

    public BlogImageMigrationHostedService(
        IServiceProvider serviceProvider,
        ILogger<BlogImageMigrationHostedService> logger,
        IConfiguration configuration)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
        _configuration = configuration;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var enabled = _configuration.GetValue<bool>("BlogImageMigration:RunOnStartup");
        if (!enabled)
        {
            return;
        }

        await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

        using var scope = _serviceProvider.CreateScope();
        var migrationService = scope.ServiceProvider.GetRequiredService<IBlogImageMigrationService>();

        try
        {
            var result = await migrationService.RunAsync(stoppingToken);
            _logger.LogInformation(
                "Blog image migration completed. Scanned={Scanned}, Processed={Processed}, Skipped={Skipped}, DurationMs={DurationMs}",
                result.Scanned,
                result.Processed,
                result.Skipped,
                (result.FinishedAt - result.StartedAt).TotalMilliseconds);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Blog image migration canceled.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Blog image migration failed.");
        }
    }
}

