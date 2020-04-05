using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace TaskManager.Services
{
  public class ProcessListReloadBackgroundService : BackgroundService
  {
    private readonly ProcessListService _processList;

    public ProcessListReloadBackgroundService(ProcessListService processList)
    {
      _processList = processList;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
      return Task.Run(async () =>
      {
        while (!stoppingToken.IsCancellationRequested)
        {
          try
          {
            _processList.Reload();
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
          } 
          catch (OperationCanceledException) { }
        }
      }, stoppingToken);
    }
  }
}