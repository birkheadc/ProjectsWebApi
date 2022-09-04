using ProjectsWebApi.Models;
using ProjectsWebApi.Repositories;

namespace ProjectsWebApi.Services;

public class ProjectService : IProjectService
{

    private readonly IProjectRepository repository;
    private readonly ILogger<ProjectService> logger;

    public ProjectService(IProjectRepository repository, ILogger<ProjectService> logger)
    {
        this.repository = repository;
        this.logger = logger;
    }

    public void AddProject(ProjectIncoming project)
    {
        logger.LogInformation("Adding project to database. Project:\n------------------------------\n{project}\n------------------------------", project.ToString());
        repository.Insert(ProjectConverter.ToEntity(project));
    }

    public void AddProjects(IEnumerable<ProjectIncoming> projects)
    {
        repository.InsertRange(ProjectConverter.ToEntity(projects));
    }

    public IEnumerable<Project> GetAllFavoriteProjects()
    {
        return repository.FindAllFavorites();
    }

    public IEnumerable<Project> GetAllProjects()
    {
        return repository.FindAll();
    }
}