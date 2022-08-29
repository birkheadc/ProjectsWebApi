using ProjectsWebApi.Models;

namespace ProjectsWebApi.Repositories;

public interface IProjectRepository
{
    public IEnumerable<Project> FindAll();
}