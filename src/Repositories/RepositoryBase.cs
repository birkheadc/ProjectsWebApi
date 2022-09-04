using MySql.Data.MySqlClient;

namespace ProjectsWebApi.Repositories;

public abstract class RepositoryBase
{
    protected DatabaseConnectionConfig connectionConfig;
    protected ILogger<RepositoryBase> logger;
    protected string connectionString;
    public RepositoryBase(DatabaseConnectionConfig connectionConfig, ILogger<RepositoryBase> logger)
    {
        this.connectionConfig = connectionConfig;
        this.logger = logger;
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

    ///<summary>Initializes tables based on the supplied schema. Should be called from the constructor of the individual repository/</summary>
    internal void InitializeTables(IEnumerable<TableSchema> schemas)
    {
        foreach (TableSchema schema in schemas)
        {
            InitializeTable(schema);
        }
    }

    internal void InitializeTable(TableSchema schema)
    {
        // Not sure if anything else needs to be done, at the moment "Initialize" simply calls "Create".
        try
        {
            CreateTable(schema.Schema);
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "FATAL EXCEPTION encountered while attempting to create the following table:\n------------------------------\n{schema}\n------------------------------", schema.Schema);
        }
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