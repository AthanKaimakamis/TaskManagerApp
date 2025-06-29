using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TaskManagerApp.ViewModels;

namespace TaskManagerApp;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow
{
  TasksViewModel ViewModel => (TasksViewModel)DataContext;

  public MainWindow()
  {
    DataContext = new TasksViewModel();
    Initialized += async (s, e) => await ViewModel.LoadAsync();

    InitializeComponent();
  }

  private async void Filter_SelectedDateChanged(object sender, SelectionChangedEventArgs e) =>
    await ViewModel.CommentFilterChanged();

  private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
  {
    if (e.ClickCount == 2)
      WindowState = (WindowState == WindowState.Normal)
                    ? WindowState.Maximized
                    : WindowState.Normal;
    else DragMove();
  }
}