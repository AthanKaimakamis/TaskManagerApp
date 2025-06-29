using Microsoft.Data.SqlClient;
using Newtonsoft.Json;
using System.Data;

namespace TaskManagerApp.Services.DB;

internal abstract class DbContextBase(string connectionString) : IDisposable
{
  readonly string _connectionString = connectionString;

  protected async Task<T> ExecuteWithCommand<T>(string query, CommandType type, Func<SqlCommand, Task<T>> commandFunc, params SqlParameter[] parameters)
  {
    await using var conn = new SqlConnection(_connectionString);
    await using var cmd = conn.CreateCommand();
    cmd.CommandType = type;
    cmd.CommandText = query;
    if (parameters != null && parameters.Length > 0)
      cmd.Parameters.AddRange(parameters);
    await conn.OpenAsync();
    return await commandFunc(cmd);
  }

  protected Task<int> ExecuteNonQuery(string query, CommandType type, params SqlParameter[] parameters) =>
    ExecuteWithCommand(query, type, async cmd => await cmd.ExecuteNonQueryAsync(), parameters);

  protected Task<object?> ExecuteScalar(string query, params SqlParameter[] parameters) =>
    ExecuteWithCommand(query, CommandType.Text, async cmd => await cmd.ExecuteScalarAsync(), parameters);

  protected Task<int> ExecuteScalar(string query, CommandType type, params SqlParameter[] parameters) =>
        ExecuteWithCommand(query, type, async cmd => Convert.ToInt32(await cmd.ExecuteScalarAsync()), parameters);

  protected async Task<TResult?> ExecuteScalar<TResult>(string query, params SqlParameter[] parameters) =>
    JsonConvert.DeserializeObject<TResult>(JsonConvert.SerializeObject(await ExecuteScalar(query, parameters)));

  protected Task<DataTable> ExecuteSql(string query, params SqlParameter[] parameters) =>
    ExecuteWithCommand(query, CommandType.Text, async cmd =>
    {
      await using var reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection);
      var dt = new DataTable();
      dt.Load(reader);
      return dt;
    }, parameters);

  protected async Task<IList<TResult>> ExecuteSql<TResult>(string query, params SqlParameter[] parameters) =>
    JsonConvert.DeserializeObject<IList<TResult>>(JsonConvert.SerializeObject(await ExecuteSql(query, parameters))) ?? [];
  

  public void Dispose() => GC.SuppressFinalize(this);
}
