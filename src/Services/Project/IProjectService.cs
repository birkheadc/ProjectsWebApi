using ProjectsWebApi.Models;

namespace ProjectsWebApi.Services;

public interface IProjectService
{
    public IEnumerable<Project> GetAllProjects();
    public IEnumerable<Project> GetAllFavoriteProjects();
    public void AddProject(ProjectIncoming project);
    public void AddProjects(IEnumerable<ProjectIncoming> projects);
    public void DeleteProjectById(Guid id);
    public void UpdateProject(Project project);
}