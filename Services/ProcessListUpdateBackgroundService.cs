using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace TaskManager.Services
{
  public class ProcessListUpdateBackgroundService : BackgroundService
  {
    private readonly ProcessListService _processList;

    public ProcessListUpdateBackgroundService(ProcessListService processList)
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
            _processList.Update();
            await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
          } 
          catch (OperationCanceledException) { }
        }
      }, stoppingToken);
    }
  }
}
