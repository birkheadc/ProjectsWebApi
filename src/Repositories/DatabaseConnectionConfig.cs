namespace ProjectsWebApi.Repositories;

public class DatabaseConnectionConfig
{
    public string Server { get; set; }
    public int Port { get; set; }
    public string Database { get; set; }
    public string User { get; set; }
    public string Password { get; set; }

    public string GetConnectionString()
    {
        return $"server={Server};port={Port};database={Database};user={User};password={Password}";
    }
}