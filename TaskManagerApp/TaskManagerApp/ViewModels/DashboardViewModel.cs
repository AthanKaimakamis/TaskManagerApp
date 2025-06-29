
using CommunityToolkit.Mvvm.ComponentModel;

namespace TaskManagerApp.ViewModels;
internal partial class DashboardViewModel : BaseViewModel
{
  [ObservableProperty]
  string? text;

  public DashboardViewModel()
  {
    Text = "Hello, World!";
  }
}
