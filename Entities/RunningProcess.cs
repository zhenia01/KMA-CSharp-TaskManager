using System;
using System.Diagnostics;
using System.Security.Principal;

namespace TaskManager.Entities
{
  public class RunningProcess : IDisposable
  {
    private readonly Process _process;

    private DateTime _startTime;
    private TimeSpan _startCpuUsage;

    public string Name { get; private set; }
    public int Id { get; private set; }
    public bool IsActive { get; private set; }
    public double MemoryUsage { get; private set; }
    public int ThreadCount { get; private set; }
    public string Username { get; private set; }
    public string FilePath { get; private set; }
    public DateTime StartTime { get; private set; }
    public double CpuUsage { get; private set; }
    public ProcessModuleCollection Modules { get; private set; }
    public ProcessThreadCollection Threads { get; private set; }

    public RunningProcess(Process p)
    {
      _process = p;
      _startTime = DateTime.UtcNow;
      _startCpuUsage = p.TotalProcessorTime;
      Refresh();
    }

    private double CpuUsageCalc()
    {
      var endTime = DateTime.UtcNow;
      var endCpuUsage = _process.TotalProcessorTime;

      var cpuUsedMs = (endCpuUsage - _startCpuUsage).TotalMilliseconds;
      var totalMsPassed = (endTime - _startTime).TotalMilliseconds;
      var cpuUsageTotal = cpuUsedMs / (Environment.ProcessorCount * totalMsPassed);

      _startTime = endTime;
      _startCpuUsage = endCpuUsage;

      return cpuUsageTotal * 100;
    }

    public void Refresh()
    {
      _process.Refresh();
      Username = WindowsIdentity.GetCurrent().Name;
      FilePath = _process.MainModule?.FileName;
      Threads = _process.Threads;
      Id = _process.Id;
      Name = _process.ProcessName;
      StartTime = _process.StartTime;
      MemoryUsage = _process.WorkingSet64 / 1024.0 / 1024;
      IsActive = _process.Responding;
      ThreadCount = Threads.Count;
      Modules = _process.Modules;
      CpuUsage = CpuUsageCalc();
    }

    public void Kill()
    {
      _process.Kill();
      _process.WaitForExit();
      Dispose();
    }

    #region Dispose Pattern

    private bool _disposed;

    public void Dispose()
    {
      if (_disposed) return;
      _process.Close();
      _disposed = true;
    }

    #endregion
  }
}