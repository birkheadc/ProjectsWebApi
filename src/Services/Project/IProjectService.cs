using ProjectsWebApi.Models;

namespace ProjectsWebApi.Services;

public interface IProjectService
{
    public IEnumerable<Project> GetAllProjects();
    public void AddProject(ProjectIncoming project);
    public void AddProjects(IEnumerable<ProjectIncoming> projects);
}