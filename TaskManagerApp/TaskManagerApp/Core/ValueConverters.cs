using System.Windows.Data;
using System.Globalization;
using TaskManagerApp.Services;
using TaskManagerApp.Models;
using Humanizer;

namespace TaskManagerApp.Core.ValueConverters;

public class DateFromDateTimeConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    value is DateTime dateTime ? dateTime.ToString("dd/MM/yyyy") : value;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    value is string str && DateTime.TryParse(str, out var dateTime) ? dateTime : value;
}

public class HumanizeDateConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    value is DateTime dateTime ? dateTime.Humanize(dateToCompareAgainst: DateTime.UtcNow) : value;
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    value is string str && DateTime.TryParse(str, out var dateTime) ? dateTime : value;
}

public class PickerRowValueFromIdConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
  {
    if(parameter is not ICollection<PickerRow> row)
      return "Unknown";

    return int.TryParse(value?.ToString(), out int id)
      ? row.FirstOrDefault(r => r.Id == id)?.Value ?? "Unknown"
      : "Unknown";
  }

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => 
    value is string str && parameter is ICollection<PickerRow> row
      ? row.FirstOrDefault(r => r.Value == str)?.Id.ToString() ?? "0"
      : "0";
}

public class TaskStatusConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    new PickerRowValueFromIdConverter().Convert(
      value, targetType, StoreSingletonService.Instance.TaskStatusesStore, culture);

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    new PickerRowValueFromIdConverter().ConvertBack(
      value, targetType, StoreSingletonService.Instance.TaskStatusesStore, culture);
}

public class TaskTypeConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    new PickerRowValueFromIdConverter().Convert(
      value, targetType, StoreSingletonService.Instance.TaskTypesStore, culture);
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    new PickerRowValueFromIdConverter().ConvertBack(
      value, targetType, StoreSingletonService.Instance.TaskTypesStore, culture);
}

public class CommentTypeConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    new PickerRowValueFromIdConverter().Convert(
      value, targetType, StoreSingletonService.Instance.CommentTypesStore, culture);
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    new PickerRowValueFromIdConverter().ConvertBack(
      value, targetType, StoreSingletonService.Instance.CommentTypesStore, culture);
}

public class UserDisplayNameFromIdConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    new PickerRowValueFromIdConverter().Convert(
      value, targetType, StoreSingletonService.Instance.UsersStore, culture);
  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    new PickerRowValueFromIdConverter().ConvertBack(
      value, targetType, StoreSingletonService.Instance.UsersStore, culture);
}

public class TruncateTextConverter : IValueConverter
{
  public object Convert(object? value, Type targetType, object parameter, CultureInfo culture) =>
    value?.ToString().Truncate(System.Convert.ToInt32(parameter ?? "15"), Truncator.FixedLength) ?? string.Empty;

  public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    value;
}

public class VisibiityFromNullConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    value is null ? System.Windows.Visibility.Hidden : System.Windows.Visibility.Visible;

  public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
}

public class IsNullConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    value is null;
  public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    value is bool isNull && isNull ? null : value;
}

public class IsNotNullConverter : IValueConverter
{
  public object Convert(object value, Type targetType, object parameter, CultureInfo culture) =>
    value is not null;
  public object? ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) =>
    value is bool isNotNull && isNotNull ? value : null;
}