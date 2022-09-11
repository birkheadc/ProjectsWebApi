using ProjectsWebApi.Models;

namespace ProjectsWebApi.Repositories;

public interface IProjectRepository
{
    ///<summary>Returns all Projects in the database.</summary>
    public IEnumerable<Project> FindAll(bool favoriteOnly = false);
    ///<summary>Returns all Projects where IsFavorite is True.</summary>
    public IEnumerable<Project> FindAllFavorites();
    ///<summary>Add a single Project to the database.</summary>
    public void Insert(Project project);
    ///<summary>Add multiple Projects to the database. This simply calls Insert in a foreach loop, so is not any more optimal than calling Insert manually.</summary>
    public void InsertRange(IEnumerable<Project> projects);
    ///<summary>Finds the project by ID and removes it from the database.</summary>
    public void DeleteById(Guid id);
    ///<summary>Finds the project in the database with the same ID, and updates it to match the supplied project. Does nothing if the ID has no match.</summary>
    public void Update(Project project);
}