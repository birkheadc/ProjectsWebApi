namespace ProjectsWebApi.Services;

public class ProjectService : IProjectService
{

    private readonly IProjectRepository repository;

    public ProjectService(IProjectRepository repository)
    {
        this.repository = repository;
    }

    public IEnumerable<Project> GetAllProjects()
    {
        return repository.FindAll();
    }
}