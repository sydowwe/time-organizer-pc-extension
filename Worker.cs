using ActivityLookUp;
using ActivityLookUp.DTO;
using ActivityLookUp.Processes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public class Worker : BackgroundService
{
    private readonly WindowsProcess _windowsProcess;
    private readonly ILogger<Worker> _logger;
    public Worker(IServiceProvider Services, ILogger<Worker> logger)
    {
        _logger = logger;
        _windowsProcess = Services.GetRequiredService<WindowsProcess>();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            _logger.LogInformation("Background service started");
            while (!stoppingToken.IsCancellationRequested)
            {
                await _windowsProcess.LookUp();
                await Task.Delay(5000, stoppingToken); 
            }
        }
        catch (Exception ex)
        {
            _logger.LogError($"Exception in ExecuteAsync: {ex.StackTrace}");
        }
        finally
        {
            _logger.LogInformation("Background service stopping.");
        }
    }

}
