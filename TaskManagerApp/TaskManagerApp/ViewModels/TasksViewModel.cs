using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using TaskManagerApp.Models.DTO;
using TaskManagerApp.Services.DB;
using TaskManagerApp.Windows;

namespace TaskManagerApp.ViewModels;
public partial class TasksViewModel : BaseViewModel
{
  public ObservableCollection<TaskDto> Tasks { get; } = [];
  public ObservableCollection<CommentDto> Comments { get; } = [];

  [ObservableProperty]
  TaskDto? _selectedTask;
  [ObservableProperty]
  CommentDto? _selectedComment;
  [ObservableProperty]
  string? _searchText;
  [ObservableProperty]
  CommentFilterDto commentFilter = new();
  [ObservableProperty]
  bool _isGlobalSearch = true;

  DispatcherTimer _searchTimer = new()
  {
    Interval = TimeSpan.FromMilliseconds(300),
    IsEnabled = false
  };
  bool lookingForTaskFromComment = false;

  public TasksViewModel()
  {
    _searchTimer.Tick += async (s, e) =>
    {
      _searchTimer.Stop();
      await CommentFilterChanged();
    };
  }

  [RelayCommand]
  public Task CommentFilterChanged()
  {
    if (lookingForTaskFromComment)
      return Task.CompletedTask;

    if (IsGlobalSearch)
      SelectedTask = null;

    return LoadComments();
  }

  //async Task SearchComment()
  //{
  //  if(string.IsNullOrWhiteSpace(SearchText) && SelectedTask is null)
  //    Comments.Clear();
  //  SelectedTask = null;
  //  if (string.IsNullOrWhiteSpace(SearchText))
  //    return;
  //  try
  //  {
  //    var result = await TasksContextService.Execute(c => c.GetComments(f => f.Comment = SearchText));
  //    foreach (var comment in result)
  //      Comments.Add(comment);
  //  }
  //  catch (Exception ex)
  //  {
  //    MessageBox.Show($"Error searching comments: {ex.Message}", "Search Error", MessageBoxButton.OK, MessageBoxImage.Error);
  //  }
  //}

  [RelayCommand]
  void DoubleClick(object? param)
  {
    if (param is TaskDto task)
    {
      // Handle task double click
      return;
    }

    if (param is CommentDto comment)
    {
      if (SelectedTask?.Id != SelectedComment?.TaskId)
      {
        lookingForTaskFromComment = true;
        var commStore = SelectedComment?.Id;
        SelectedTask = Tasks.First(t => t.Id == SelectedComment?.TaskId);
        lookingForTaskFromComment = false;
        return;
      }
    }
  }



  [RelayCommand]
  void NewTask() => GetTaskForm().ShowDialog();
  [RelayCommand]
  void EditTask() => GetTaskForm(SelectedTask).ShowDialog();
  [RelayCommand]
  async Task DeleteTask()
  {
    try
    {
      var res = MessageBox.Show("Are you sure you want to delete this task?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      if (res == MessageBoxResult.No)
        return;
      await TasksContextService.Execute(c => c.DeleteTask(SelectedTask!.Id));
      await LoadAsync();
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Error deleting task: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
      return;
    }
  }
  [RelayCommand]
  void NewComment() => GetCommentForm(new CommentDto { TaskId = SelectedTask?.Id }).ShowDialog();
  [RelayCommand]
  void EditComment() => GetCommentForm(SelectedComment).ShowDialog();
  [RelayCommand]
  async Task DeleteComment()
  {
    try
    {
      var res = MessageBox.Show("Are you sure you want to delete this task?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning);
      if (res == MessageBoxResult.No)
        return;
      await TasksContextService.Execute(c => c.DeleteComment(SelectedComment!.Id));
      await LoadComments();
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Error deleting task: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
      return;
    }
  }

  Window GetTaskForm(TaskDto? model = null)
  {
    var form = new FormWindow(typeof(TaskDto), model);
    ((TaskFormViewModel)form.DataContext).OnSaved += async (_, _) =>
    {
      await LoadAsync();
      SelectedTask = Tasks.FirstOrDefault(t => t.Id == model?.Id);
    };
    return form;
  }
  Window GetCommentForm(CommentDto? model = null)
  {
    var form = new FormWindow(typeof(CommentDto), model);
    ((CommentFormViewModel)form.DataContext).OnSaved += async (_, _) =>
    {
      await LoadComments();
      SelectedComment = Comments.FirstOrDefault(c => c.Id == model?.Id);
    };
    return form;
  }


  public async Task LoadAsync()
  {
    try
    {
      var tasks = await TasksContextService.Execute(c => c.GetTasks());
      if (Tasks.Any()) Tasks.Clear();
      foreach (var task in tasks) Tasks.Add(task);
    }
    catch (Exception ex)
    {
      MessageBox.Show($"Error loading tasks: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
    }
  }

  async Task LoadComments()
  {
    try
    {
      var result = await TasksContextService.Execute(c => c.GetComments(() => CommentFilter));
      Comments.Clear();
      foreach (var comment in result)
        Comments.Add(comment);
    }
    catch (Exception)
    {
      MessageBox.Show("Error applying comment filter", "Filter Error", MessageBoxButton.OK, MessageBoxImage.Error);
      return;
    }
  }

  protected override async void OnPropertyChanged(PropertyChangedEventArgs e)
  {
    base.OnPropertyChanged(e);

    switch (e.PropertyName)
    {
      case nameof(SelectedTask):
        ResetFilter(lookingForTaskFromComment);
        if (SelectedTask is not null)
          IsGlobalSearch = false;
        CommentFilter.TaskId = SelectedTask?.Id;
        await LoadComments();
        return;

      case nameof(SelectedComment):
        // TODO: Handle selected comment change if needed

        return;

      case nameof(SearchText):
        _searchTimer.Stop();
        _searchTimer.Start();
        CommentFilter.Comment = SearchText;
        return;
    }
  }

  void ResetFilter(bool notify = false)
  {
    CommentFilter.TaskId = null;
    CommentFilter.DateAdded = null;
    CommentFilter.Comment = null;
    CommentFilter.TypeId = null;
    CommentFilter.ReminderDate = null;
    IsGlobalSearch = true;

    if (notify)
      OnPropertyChanged(nameof(CommentFilter));
  }
}
