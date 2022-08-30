using ProjectsWebApi.Models;
using ProjectsWebApi.Repositories;

namespace ProjectsWebApi.Services;

public class ProjectService : IProjectService
{

    private readonly IProjectRepository repository;

    public ProjectService(IProjectRepository repository)
    {
        this.repository = repository;
    }

    public void AddProject(ProjectIncoming project)
    {
        repository.Insert(project);
    }

    public void AddProjects(IEnumerable<ProjectIncoming> projects)
    {
        repository.InsertRange(projects);
    }

    public IEnumerable<Project> GetAllProjects()
    {
        return repository.FindAll();
    }
}