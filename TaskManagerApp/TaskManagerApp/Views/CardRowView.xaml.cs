using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TaskManagerApp.Views;
/// <summary>
/// Interaction logic for CardRowView.xaml
/// </summary>
public partial class CardRowView : UserControl
{
  public static readonly DependencyProperty DoubleClickCommandProperty =
      DependencyProperty.Register(
        nameof(DoubleClickCommand),
        typeof(ICommand),
        typeof(CardRowView),
        new PropertyMetadata(null));
  public ICommand? DoubleClickCommand
  {
    get => (ICommand?)GetValue(DoubleClickCommandProperty);
    set => SetValue(DoubleClickCommandProperty, value);
  }

  public static readonly DependencyProperty CornerRadiusProperty =
      DependencyProperty.Register(
        nameof(CornerRadius),
        typeof(CornerRadius),
        typeof(CardRowView),
        new FrameworkPropertyMetadata(
          new CornerRadius(0),
            FrameworkPropertyMetadataOptions.AffectsRender |
            FrameworkPropertyMetadataOptions.AffectsMeasure));
  public CornerRadius CornerRadius
  {
    get => (CornerRadius)GetValue(CornerRadiusProperty);
    set => SetValue(CornerRadiusProperty, value);
  }

  public CardRowView()
  {
    MouseDoubleClick += (s, _) =>
    {
      var param = (s as CardRowView)?.DataContext;
      if (DoubleClickCommand?.CanExecute(param) ?? false)
        DoubleClickCommand.Execute(param);
    };

    InitializeComponent();
  }
}
