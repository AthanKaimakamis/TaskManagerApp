using System.Windows;
using TaskManagerApp.Models.DTO;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp.Windows;
/// <summary>
/// Interaction logic for FormWindow.xaml
/// </summary>
public partial class FormWindow : Window
{
  public FormWindow(Type type, object? model = null)
  {
    switch(type)
    {
      case Type t when t == typeof(TaskDto):
        DataContext = new TaskFormViewModel(model as TaskDto);
        Title = string.Format("{0} Task", model is null ? "New" : "Edit");
        break;
      case Type t when t == typeof(CommentDto):
        DataContext = new CommentFormViewModel(model as CommentDto);
        Title = string.Format("{0} Comment", (model as CommentDto)?.TaskId > 0 ? "New" : "Edit");
        break;
      default:
        throw new ArgumentException("Unsupported type for FormWindow", nameof(type));
    }

    InitializeComponent();
  }
}
