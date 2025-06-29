using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel.DataAnnotations;
using TaskManagerApp.Windows;

namespace TaskManagerApp.ViewModels;
public abstract partial class BaseFormViewModel<TModel> : BaseViewModel
{
  public bool IsNew => Model is null;
  [ObservableProperty]
  TModel _model;

  public event EventHandler? OnSaved;

  [RelayCommand]
  public virtual async Task Save()
  {
    OnSaved?.Invoke(this, EventArgs.Empty);
    Close();
  }

  [RelayCommand]
  public virtual void Close() =>
    App.Current.Windows.OfType<FormWindow>().First().Close();

  protected BaseFormViewModel([Required] TModel model)
  {
    Model = model;
  }

}
