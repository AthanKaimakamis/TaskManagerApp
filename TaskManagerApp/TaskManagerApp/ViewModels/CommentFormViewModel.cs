using System.Windows;
using TaskManagerApp.Models.DTO;
using TaskManagerApp.Services.DB;

namespace TaskManagerApp.ViewModels;
public partial class CommentFormViewModel(CommentDto? model) : BaseFormViewModel<CommentDto>(model ?? new())
{
  public override async Task Save()
  {
    try
    {
      if (Model?.Id == 0)
        Model.Id = await TasksContextService.Execute(c => c.CreateComment(Model));
      else
        await TasksContextService.Execute(c => c.UpdateComment(Model));

      await base.Save();
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Error saving command: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }
}
