namespace TaskManagerApp.Models.DTO;
public class CommentDto
{
  public int Id { get; set; }
  public int? TaskId { get; set; }
  public DateTime? DateAdded { get; set; }
  public string? Comment { get; set; }
  public int? TypeId { get; set; }
  public DateTime? ReminderDate { get; set; }
}