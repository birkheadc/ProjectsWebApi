using MySql.Data.MySqlClient;

namespace ProjectsWebApi.Repositories;

public class PasswordRepository : RepositoryBase, IPasswordRepository
{
  public PasswordRepository(DatabaseConnectionConfig connectionConfig, TableSchemasConfiguration tableSchemasConfiguration, ILogger<ProjectRepository> logger) : base(connectionConfig, logger)
  {
    InitializeTables(tableSchemasConfiguration.TableSchemas["Password"]);
  }

  public string GetPassword()
  {
    using (MySqlConnection connection = GetConnection())
    {
      MySqlCommand command = new();
      command.Connection = connection;
      command.CommandText = "SELECT * FROM password LIMIT 1;";
    }
    return "null";
  }

  public void SetPassword(string password)
  {
    using (MySqlConnection connection = GetConnection())
    {
      MySqlCommand command = new();
      command.Connection = connection;
      command.CommandText = "DELETE FROM password;";
    } 
  }
}