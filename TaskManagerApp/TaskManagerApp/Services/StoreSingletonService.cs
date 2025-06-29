using CommunityToolkit.Mvvm.ComponentModel;
using TaskManagerApp.Models;
using TaskManagerApp.Services.DB;

namespace TaskManagerApp.Services;
public partial class StoreSingletonService : ObservableObject
{
  static readonly Lazy<StoreSingletonService> _instance =
    new(new StoreSingletonService());

  public static StoreSingletonService Instance => _instance.Value;

  [ObservableProperty]
  ICollection<PickerRow> taskStatusesStore = [];
  [ObservableProperty]
  ICollection<PickerRow> taskTypesStore = [];
  [ObservableProperty]
  ICollection<PickerRow> commentTypesStore = [];
  [ObservableProperty]
  ICollection<PickerRow> usersStore = [];

  public async Task Init()
  {
    await UpdateTaskStatuses();
    await UpdateTaskTypes();
    await UpdateCommentTypes();
    await UpdateUsers();
  }

  public async Task UpdateTaskStatuses()
  {
    TaskStatusesStore = await TasksContextService.Execute(c => c.GetTaskStatuses());
    OnPropertyChanged(nameof(TaskStatusesStore));
  }
  public async Task UpdateTaskTypes()
  {
    var result = await TasksContextService.Execute(c => c.GetTaskTypes());
    TaskTypesStore = [.. result.Prepend(new() { Id = -1, Value = "None" })];
    OnPropertyChanged(nameof(TaskTypesStore));
  }
  public async Task UpdateCommentTypes()
  {
    var result = await TasksContextService.Execute(c => c.GetCommentTypes());
    CommentTypesStore = [.. result.Prepend(new() { Id = -1, Value = "None" })];
    OnPropertyChanged(nameof(CommentTypesStore));
  }
  public async Task UpdateUsers()
  {
    var result = await TasksContextService.Execute(c => c.GetUsers());
    UsersStore = [.. result.Prepend(new() { Id = -1, Value = "None" })];
    OnPropertyChanged(nameof(UsersStore));
  }
}
