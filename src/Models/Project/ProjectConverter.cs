namespace ProjectsWebApi.Models;

public static class ProjectConverter
{
    public static Project ToEntity(ProjectIncoming incoming)
    {
        Project project = new()
        {
            Id = Guid.NewGuid(),
            Name = incoming.Name,
            ShortDescription = incoming.ShortDescription,
            LongDescription = incoming.LongDescription,
            Technologies = incoming.Technologies,
            Site = incoming.Site,
            Source = incoming.Source,
            IsFavorite = incoming.IsFavorite
        };
        return project;
    }

    public static IEnumerable<Project> ToEntity(IEnumerable<ProjectIncoming> incomings)
    {
        List<Project> projects = new();
        foreach (ProjectIncoming incoming in incomings)
        {
            projects.Add(ToEntity(incoming));
        }
        return projects;
    }
}