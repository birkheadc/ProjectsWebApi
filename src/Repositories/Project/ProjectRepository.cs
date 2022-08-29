using ProjectsWebApi.Models;

namespace ProjectsWebApi.Repositories;

public class ProjectRepository : RepositoryBase, IProjectRepository
{
    public ProjectRepository(DatabaseConnectionConfig connectionConfig, TableSchemasConfiguration tableSchemasConfiguration) : base(connectionConfig)
    {
        foreach (TableSchema schema in tableSchemasConfiguration.TableSchemas["Projects"])
        {
            InitializeTable(schema);
        }
    }

    public IEnumerable<Project> FindAll()
    {
        List<Project> projects = new();

        // Todo

        return projects;
    }
}