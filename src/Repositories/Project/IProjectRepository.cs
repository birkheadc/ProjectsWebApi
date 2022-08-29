namespace ProjectsWebApi.Repositories;

public interface IProjectRepository
{
    public IEnumerable<Project> FindAll();
}