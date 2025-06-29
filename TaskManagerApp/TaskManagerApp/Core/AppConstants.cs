namespace TaskManagerApp.Core;
internal class AppConstants
{
  internal class Db_Settings
  {

// Intentionaly left like that to attract the tester
// Enter your DB Connection
    const string SERVER = ;
    const string DATABASE = ;
    const string USER_ID = ;
    const string PASSWORD = ;
    const string TRUST_SERVER_CERTIFICATE = "True";

    internal static string ConnectionString()
    {
      if (string.IsNullOrWhiteSpace(SERVER)
        || string.IsNullOrWhiteSpace(DATABASE)
        || string.IsNullOrWhiteSpace(USER_ID)
        || string.IsNullOrWhiteSpace(PASSWORD))
        throw new InvalidOperationException("Connection to database not configured properly");

      return string.Format("Server={0};Database={1};User Id={2};Password='{3}';TrustServerCertificate={4};",
        SERVER, DATABASE, USER_ID, PASSWORD, TRUST_SERVER_CERTIFICATE);
    }
  }
}