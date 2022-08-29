namespace ProjectsWebApi.Repositories;

public class ProjectRepository : RepositoryBase, IProjectRepository
{
    public IEnumerable<Project> FindAll()
    {
        List<Project> projects = new();

        // Todo

        return projects;
    }
}