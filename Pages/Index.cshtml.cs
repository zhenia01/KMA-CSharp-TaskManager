using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TaskManager.Services;

namespace TaskManager.Pages
{
  public class IndexModel : PageModel
  {
    public ProcessListService ProcessListService { get; }

    public IndexModel(ProcessListService processList)
    {
      ProcessListService = processList;
    }

    public IActionResult OnGetProcessList()
    {
      return Partial("ProcessTableBodyPartial", ProcessListService.Processes);
    }

    public async Task<IActionResult> OnGetSortAsync(int index)
    {
      if (index >= 0 || index <= 9)
      {
        await ProcessListService.SortAsync((SortingOption) index);
      }

      return OnGetProcessList();
    }

    public IActionResult OnGetRemoveById(int id)
    {
      ProcessListService.RemoveProcessById(id);

      return OnGetProcessList();
    }

    public IActionResult OnGetOpenFolder(int id)
    {
      ProcessListService.OpenFolderById(id);

      return OnGetProcessList();
    }

    public IActionResult OnGetModuleList(int id)
    {
      var m = ProcessListService.GetModulesById(id);
      if (m != null)
      {
        var p = ProcessListService.GetProcessById(id);
        return Partial("ModuleListPartial", ($"{p.Name}({p.Id})", m));
      }

      return NotFound();
    }
    
    public IActionResult OnGetThreadList(int id)
    {
      var t = ProcessListService.GetThreadsById(id);
      if (t != null)
      {
        var p = ProcessListService.GetProcessById(id);
        return Partial("ThreadListPartial", ($"{p.Name}({p.Id})", t));
      }

      return NotFound();
    }
  }
}