namespace TaskManagerApp.Models.DTO;

public class CommentFilterDto : CommentDto
{
  public int Skip { get; set; } = 0;
  public int Take { get; set; } = 50;
}
