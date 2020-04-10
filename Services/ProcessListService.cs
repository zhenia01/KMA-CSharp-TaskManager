using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Entities;

namespace TaskManager.Services
{
  public enum SortingOption
  {
    Name,
    Id,
    IsActive,
    CpuUsage,
    MemoryUsage,
    ThreadCount,
    UserName,
    FilePath,
    StartTime
  }

  public class ProcessListService : IDisposable
  {
    private readonly object _locker = new object();

    public IReadOnlyList<RunningProcess> Processes => _processes;

    private List<RunningProcess> _processes;
    private (SortingOption option, bool isAsc) _lastOption;

    public ProcessListService()
    {
      Reload();
    }

    public void Reload()
    {
      lock (_locker)
      {
        _processes = new List<RunningProcess>(
          Process.GetProcesses().Where(p =>
          {
            try
            {
              var unused = p.MainModule; // checking if process can be read
              return true;
            }
            catch (Win32Exception)
            {
              return false;
            }
            catch (InvalidOperationException)
            {
              return false;
            }
          }).Select(p => new RunningProcess(p)));
      }

      SortAsync().Wait();
    }

    public void Update()
    {
      lock (_locker)
      {
        _processes = new List<RunningProcess>(
          _processes.Where(p =>
          {
            try
            {
              p.Refresh();
              return true;
            }
            catch (InvalidOperationException)
            {
              return false;
            }
            catch (Win32Exception)
            {
              return false;
            }
          }));
      }

      SortAsync().Wait();
    }

    public async Task SortAsync(SortingOption option)
    {
      if (option == _lastOption.option)
      {
        _lastOption.isAsc = !_lastOption.isAsc;
      }
      else
      {
        _lastOption.option = option;
      }

      await SortAsync();
    }

    private Task SortAsync()
    {
      return Task.Run(() =>
      {
        lock (_locker)
        {
          _processes.Sort((p1, p2) =>
          {
            int result = _lastOption.option switch
            {
              SortingOption.Name => p1.Name.CompareTo(p2.Name),
              SortingOption.Id => p1.Id.CompareTo(p2.Id),
              SortingOption.IsActive => p1.IsActive.CompareTo(p2.IsActive),
              SortingOption.CpuUsage => p1.CpuUsage.CompareTo(p2.CpuUsage),
              SortingOption.MemoryUsage => p1.MemoryUsage.CompareTo(p2.MemoryUsage),
              SortingOption.ThreadCount => p1.ThreadCount.CompareTo(p2.ThreadCount),
              SortingOption.UserName => p1.Username.CompareTo(p2.Username),
              SortingOption.FilePath => p1.FilePath.CompareTo(p2.FilePath),
              SortingOption.StartTime => p1.StartTime.CompareTo(p2.StartTime),
              _ => -1
            };
            return _lastOption.isAsc ? result : -result;
          });
        }
      });
    }

    public RunningProcess GetProcessById(int id)
    {
      return _processes.Find(pr => pr.Id == id);
    }

    public void RemoveProcessById(int id)
    {
      lock (_locker)
      {
        var p = _processes.Find(pr => pr.Id == id);
        if (p != null)
        {
          p.Kill();
          _processes.Remove(p);
        }
      }
    }

    public void OpenFolderById(int id)
    {
      lock (_locker)
      {
        var p = GetProcessById(id);
        if (p != null)
        {
          Process.Start("explorer.exe", $"/open, {Directory.GetParent(p.FilePath).FullName}");
        }
      }
    }

    public ProcessModuleCollection GetModulesById(int id)
    {
      var p = GetProcessById(id);
      return p?.Modules;
    }

    public ProcessThreadCollection GetThreadsById(int id)
    {
      var p = GetProcessById(id);
      return p?.Threads;
    }

    #region Dispose Pattern

    private bool _disposed;

    public void Dispose()
    {
      if (_disposed) return;
      foreach (var runningProcess in Processes)
      {
        runningProcess.Dispose();
      }

      _disposed = true;
    }

    #endregion
  }
}