using Microsoft.Data.SqlClient;
using System.Data;
using System.Text;
using TaskManagerApp.Core;
using TaskManagerApp.Models;
using TaskManagerApp.Models.DTO;

namespace TaskManagerApp.Services.DB;

internal class TasksContextService()
  : DbContextBase(AppConstants.Db_Settings.ConnectionString())
{
  public static async Task<T> Execute<T>(Func<TasksContextService, Task<T>> func)
  {
    using TasksContextService db = new();
    return await func.Invoke(db);
  }

  public static Task Execute(Action<TasksContextService> func)
  {
    using TasksContextService db = new();
    func.Invoke(db);
    return Task.CompletedTask;
  }

  private T GetOutParamValue<T>(IList<SqlParameter> parameters, string paramName) =>
    (T)parameters
    .Where(p => p.Direction == ParameterDirection.Output)
    .First(p => p.ParameterName == paramName).Value;

  public Task<IList<PickerRow>> GetUsers() =>
      ExecuteSql<PickerRow>("SELECT Id, DisplayName AS Value FROM Users", []);

  public async Task<int> CreateUser(string displayName)
  {
    SqlParameter[] parameters = [
      new("@DisplayName", displayName),
      new ("@NewId", SqlDbType.Int) { Direction = ParameterDirection.Output }
      ];
    await ExecuteScalar("CreateUser", CommandType.StoredProcedure, parameters);
    return GetOutParamValue<int>(parameters, "@NewId");
  }

  public Task UpdateUser(int id, string displayName) =>
    ExecuteNonQuery("UpdateUser", CommandType.StoredProcedure, [new("@Id", id), new("@DisplayName", displayName)]);

  public Task DeleteUser(int id) =>
    ExecuteNonQuery("DeleteUser", CommandType.StoredProcedure, [new("@Id", id)]);

  public Task<IList<PickerRow>> GetTaskStatuses() =>
      ExecuteSql<PickerRow>("SELECT Id, [Name] AS Value FROM TaskStatuses", []);

  public Task<IList<PickerRow>> GetTaskTypes() =>
      ExecuteSql<PickerRow>("SELECT Id, [Name] AS Value FROM TaskTypes", []);

  public Task<IList<TaskDto>> GetTasks() =>
    ExecuteSql<TaskDto>("SELECT * FROM Tasks ORDER BY DateAdded DESC", []);

  public Task<TaskDto?> GetTask(int id) =>
    ExecuteScalar<TaskDto>("SELECT * FROM Tasks WHERE Id = @Id", [new SqlParameter("@Id", id)]);

  public async Task<int> CreateTask(TaskDto task)
  {
    SqlParameter[] parameters = [
      new SqlParameter("@RequiredByDate", task.RequiredByDate ?? (object)DBNull.Value),
      new SqlParameter("@Description", task.Description ?? (object)DBNull.Value),
      new SqlParameter("@StatusId", task.StatusId ?? (object)DBNull.Value),
      new SqlParameter("@TypeId", task.TypeId ?? (object)DBNull.Value),
      new SqlParameter("@UserId", task.UserId ?? (object)DBNull.Value),
      new SqlParameter("@NewId", SqlDbType.Int) { Direction = ParameterDirection.Output }
    ]; 
    await ExecuteNonQuery("CreateTask", CommandType.StoredProcedure, parameters);
    return GetOutParamValue<int>(parameters, "@NewId");
  }

  public Task UpdateTask(TaskDto task) =>
    ExecuteNonQuery("UpdateTask", CommandType.StoredProcedure, [
      new SqlParameter("@Id", task.Id),
      new SqlParameter("@RequiredByDate", task.RequiredByDate ?? (object)DBNull.Value),
      new SqlParameter("@Description", task.Description ?? (object)DBNull.Value),
      new SqlParameter("@StatusId", task.StatusId ?? (object)DBNull.Value),
      new SqlParameter("@TypeId", task.TypeId ?? (object)DBNull.Value),
      new SqlParameter("@UserId", task.UserId ?? (object)DBNull.Value)
    ]);

  public Task DeleteTask(int id) =>
    ExecuteNonQuery("DeleteTask", CommandType.StoredProcedure, [new SqlParameter("@Id", id)]);

  public Task<IList<PickerRow>> GetCommentTypes() =>
      ExecuteSql<PickerRow>("SELECT Id, [Name] AS Value FROM CommentTypes", []);

  public Task<IList<CommentDto>> GetComments(Func<CommentFilterDto>? filter = null)
  {
    List<SqlParameter> ps = [];
    StringBuilder sql = new("SELECT * FROM Comments WHERE 1=1");

    CommentFilterDto f = filter?.Invoke() ?? new();
   ;
    if (f.Id > 0)
    {
      sql.Append(" AND Id = @Id");
      ps.Add(new SqlParameter("@Id", f.Id));
    }
    if (f.TaskId > 0)
    {
      sql.Append(" AND TaskId = @taskId");
      ps.Add(new SqlParameter("@taskId", f.TaskId));
    }
    if (!string.IsNullOrEmpty(f.Comment))
    {
      sql.Append(" AND Comment LIKE @comment");
      ps.Add(new SqlParameter("@comment", $"%{f.Comment}%"));
    }
    if (f.TypeId > 0)
    {
      sql.Append(" AND TypeId = @type");
      ps.Add(new SqlParameter("@type", f.TypeId));
    }
    if (f.DateAdded != default)
    {
      sql.Append(" AND DateAdded = @dateAdded");
      ps.Add(new SqlParameter("@dateAdded", f.DateAdded?.Date));
    }
    if (f.ReminderDate.HasValue)
    {
      sql.Append(" AND ReminderDate = @reminderDate");
      ps.Add(new SqlParameter("@reminderDate", f.ReminderDate.Value));
    }


    sql.Append(" ORDER BY DateAdded DESC");
    //sql.Append(" OFFSET @Skip ROWS FETCH NEXT @Take ROWS ONLY");
    //ps.Add(new SqlParameter("@Skip", f.Skip));
    //ps.Add(new SqlParameter("@Take", f.Take));

    return ExecuteSql<CommentDto>(sql.ToString(), [.. ps]);
  }

  public async Task<int> CreateComment(CommentDto comment)
  {
    SqlParameter[] parameters = [
      new SqlParameter("@TaskId", comment.TaskId),
      new SqlParameter("@CommentText", comment.Comment ?? (object)DBNull.Value),
      new SqlParameter("@TypeId", comment.TypeId ?? (object)DBNull.Value),
      new SqlParameter("@ReminderDate", comment.ReminderDate ?? (object)DBNull.Value),
      new SqlParameter("@NewId", SqlDbType.Int) { Direction = ParameterDirection.Output }
    ];
    await ExecuteNonQuery("CreateComment", CommandType.StoredProcedure, parameters);
    return GetOutParamValue<int>(parameters, "@NewId");
  }

  public Task UpdateComment(CommentDto comment) =>
    ExecuteNonQuery("UpdateComment", CommandType.StoredProcedure, [
      new SqlParameter("@Id", comment.Id),
      new SqlParameter("@CommentText", comment.Comment ?? (object)DBNull.Value),
      new SqlParameter("@TypeId", comment.TypeId ?? (object)DBNull.Value),
      new SqlParameter("@ReminderDate", comment.ReminderDate ?? (object)DBNull.Value)
    ]);

  public Task DeleteComment(int id) =>
    ExecuteNonQuery("DeleteComment", CommandType.StoredProcedure, [new SqlParameter("@Id", id)]);
}
