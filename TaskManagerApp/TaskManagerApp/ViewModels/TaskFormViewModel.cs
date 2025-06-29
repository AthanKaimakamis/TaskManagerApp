using System.Windows;
using TaskManagerApp.Models.DTO;
using TaskManagerApp.Services.DB;

namespace TaskManagerApp.ViewModels;
public partial class TaskFormViewModel(TaskDto? model) : BaseFormViewModel<TaskDto>(model ?? new())
{
  public override async Task Save()
  {
    try
    {
      if (Model?.Id == 0)
        Model.Id = await TasksContextService.Execute(c => c.CreateTask(Model));
      else
        await TasksContextService.Execute(c => c.UpdateTask(Model!));

      await base.Save();
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Error saving task: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }
}
