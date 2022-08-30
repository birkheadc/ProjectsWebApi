using System.Text;
using MySql.Data.MySqlClient;
using ProjectsWebApi.Models;

namespace ProjectsWebApi.Repositories;

public class ProjectRepository : RepositoryBase, IProjectRepository
{
    public ProjectRepository(DatabaseConnectionConfig connectionConfig, TableSchemasConfiguration tableSchemasConfiguration) : base(connectionConfig)
    {
        InitializeTables(tableSchemasConfiguration.TableSchemas["Projects"]);
    }

    public IEnumerable<Project> FindAll()
    {
        List<Project> projects = new();

        // Todo

        return projects;
    }

    ///<summary>Inserts the Project into the database. Because there is a many-to-many relationship between Projects and Technologies, this requires a few steps.</summary>
    public void Insert(ProjectIncoming project)
    {
        Console.WriteLine("Attempting to insert project...");
        InsertProject(project);
        if (project.Technologies.Length > 0)
        {
            InsertTechnologies(project.Technologies);
            Join(project);
        }
    }

    public void InsertRange(IEnumerable<ProjectIncoming> projects)
    {
        foreach (ProjectIncoming project in projects) Insert(project);
    }
    ///<summary>Adds technologies to the technologies table database if they don't already exist.</summary>
    private void InsertTechnologies(string[] technologies)
    {
        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            StringBuilder sb = new();
            sb.Append("INSERT IGNORE INTO technologies (name) VALUES ");

            // Todo: Comment / refactor this code
            MySqlParameter[] parameters = new MySqlParameter[technologies.Length];
            for (int i = 0; i < technologies.Length; i++)
            {
                string p = "@" + i;
                MySqlParameter parameter = new(p, technologies[i]);
                parameters[i] = parameter;

                sb.Append("(" + p + "), ");
            }
            sb.Length -= 2;
            command.Parameters.AddRange(parameters);
            command.CommandText = sb.ToString();
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    ///<summary>Adds the project to the projects table.</summary>
    private void InsertProject(ProjectIncoming project)
    {
        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = $"INSERT INTO projects (name, short_description, long_description, site, source) values (@name, @shortDesc, @longDesc, @site, @source)";
            command.Parameters.AddWithValue("@name", project.Name);
            command.Parameters.AddWithValue("@shortDesc", project.ShortDescription);
            command.Parameters.AddWithValue("@longDesc", project.LongDescription);
            command.Parameters.AddWithValue("@site", project.Site);
            command.Parameters.AddWithValue("@source", project.Source);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    ///<summary>Joins the technologies and projects tables on the project_technologies table</summary>
    private void Join(ProjectIncoming project)
    {
        // Todo
    }
}