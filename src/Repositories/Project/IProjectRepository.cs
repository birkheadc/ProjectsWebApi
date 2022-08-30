using ProjectsWebApi.Models;

namespace ProjectsWebApi.Repositories;

public interface IProjectRepository
{
    ///<summary>Returns all Projects in the database.</summary>
    public IEnumerable<Project> FindAll();
    ///<summary>Add a single Project to the database.</summary>
    public void Insert(ProjectIncoming project);
    ///<summary>Add multiple Projects to the database. This simply calls Insert in a foreach loop, so is not any more optimal than calling Insert manually.</summary>
    public void InsertRange(IEnumerable<ProjectIncoming> projects);
}