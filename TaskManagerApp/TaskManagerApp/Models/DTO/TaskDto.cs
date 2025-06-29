namespace TaskManagerApp.Models.DTO;
public class TaskDto
{
  public int Id { get; set; }
  public DateTime? DateAdded { get; set; }
  public DateTime? RequiredByDate { get; set; }
  public string? Description { get; set; }
  public int? StatusId { get; set; }
  public int? TypeId { get; set; }
  public int? UserId { get; set; }
  public DateTime? NextActionDate { get; set; }
}
