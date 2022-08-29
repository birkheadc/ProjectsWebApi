namespace ProjectsWebApi.Services;

public interface IProjectService
{
    public IEnumerable<Project> GetAllProjects();
}