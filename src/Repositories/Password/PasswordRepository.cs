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
      connection.Open();
      using (MySqlDataReader reader = command.ExecuteReader())
      {
        if (reader.Read())
        {
          return reader["password"].ToString();
        }
        return "";
      }
    }
  }

  public void SetPassword(string password)
  {
    using (MySqlConnection connection = GetConnection())
    {
      MySqlCommand command = new();
      command.Connection = connection;
      command.CommandText = "INSERT INTO password (password) VALUES (@password);";
      command.Parameters.AddWithValue("@password", password);
      
      connection.Open();
      command.ExecuteNonQuery();
      connection.Close();
    } 
  }

  private void ClearPassword()
  {
    using (MySqlConnection connection = GetConnection())
    {
      MySqlCommand command = new();
      command.Connection = connection;
      command.CommandText = "DELETE FROM password;";

      connection.Open();
      command.ExecuteNonQuery();
      connection.Close();
    } 
  }
}