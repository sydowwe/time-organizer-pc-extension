using ActivityLookUp.DTO;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActivityLookUp.Processes
{
    public class WindowsProcess
    {
        private readonly ILogger<WindowsProcess> _logger;
        private readonly AppSettings AppSettings;
        private readonly Npsql.NpsqlRepository NpsqlRepository;

        public WindowsProcess(IServiceProvider Services, ILogger<WindowsProcess> logger)
        {
            _logger = logger;
            AppSettings = Services.GetRequiredService<AppSettings>();
            NpsqlRepository = Services.GetRequiredService<Npsql.NpsqlRepository>();
        }
        public async Task LookUp()
        {
            Process[] processList = Process.GetProcesses().Where(e => !string.IsNullOrEmpty(e.MainWindowTitle)).ToArray();
            foreach (Process process in processList)
            {
                try
                {
                    if (!string.IsNullOrEmpty(process.MainWindowTitle) && this.AppSettings.Apps != null)
                    {

                        if (this.AppSettings.Apps.Contains(process.ProcessName))
                        {
                            await Validate(process);
                            this._logger.LogInformation($"Process ID: {process.Id}, Name: {process.ProcessName}, Title: {process.MainWindowTitle}, Time : {process.StartTime}");
                        }

                    }
                }
                catch (Exception e)
                {
                    this._logger.LogError($"Could not retrieve information for process ID: {process.Id}. Stack: {e.StackTrace}");

                }
            }
        }


        private async Task Validate(Process process)
        {
            if (!await NpsqlRepository.IdExists(process.ProcessName, process.MainWindowTitle, process.StartTime.ToUniversalTime()))
            {
                await NpsqlRepository.AddPCActivityAsync(new PCActivity()
                {
                    ProcessName = process.ProcessName,
                    MainWindowTitle = process.MainWindowTitle,
                    Duration = 0,
                    StartTimestamp = process.StartTime.ToUniversalTime(),
                    CreatedAt = DateTime.UtcNow,
                });
            }
            else
            {
                PCActivity activity = await NpsqlRepository.GetId(process.ProcessName, process.MainWindowTitle, process.StartTime.ToUniversalTime());
                activity.Duration = (DateTime.UtcNow.ToUniversalTime() - process.StartTime.ToUniversalTime()).TotalSeconds;
                activity.UpdatedAt = DateTime.UtcNow;
                await NpsqlRepository.UpdatePCActivityAsync(activity);
            }

        }
    }

}
