using MySql.Data.MySqlClient;

namespace ProjectsWebApi.Repositories;

public abstract class RepositoryBase
{
    protected DatabaseConnectionConfig connectionConfig;
    protected string connectionString;
    public RepositoryBase(DatabaseConnectionConfig connectionConfig)
    {
        // Todo: Initialize the database / confirm it is properly initialized.
        this.connectionConfig = connectionConfig;
        connectionString = connectionConfig.GetConnectionString();
    }

    internal MySqlConnection GetConnection()
    {
        try
        {
            MySqlConnection connection = new(connectionString);
            return connection;
        }
        catch
        {
            throw new Exception("Fatal Error: Could not connect to database!");
        }
    }

    internal void InitializeTable(TableSchema schema)
    {
        CreateTable(schema.Schema);
    }

    internal void CreateTable(string schema)
    {
        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.CommandText = schema;
            command.Connection = connection;
            connection.Open();
            command.ExecuteNonQuery();
        }
    }

    internal int GetCountFromScalarCommand(MySqlCommand command)
    {
        return int.Parse(command.ExecuteScalar().ToString() ?? "0");
    }
}