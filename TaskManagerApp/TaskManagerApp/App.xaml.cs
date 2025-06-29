using System.Windows;
using TaskManagerApp.Services;

namespace TaskManagerApp
{
  /// <summary>
  /// Interaction logic for App.xaml
  /// </summary>
  public partial class App : Application
  {
    protected override async void OnStartup(StartupEventArgs e)
    {
      base.OnStartup(e);

      // Forced Login for before Render
      try
      {
        await StoreSingletonService.Instance.Init();
      }
      catch (Exception ex)
      {
        MessageBox.Show($"Error initializing application: {ex.Message}", "Initialization Error", MessageBoxButton.OK, MessageBoxImage.Error);
      }

      new MainWindow().Show();
    }
  }

}
