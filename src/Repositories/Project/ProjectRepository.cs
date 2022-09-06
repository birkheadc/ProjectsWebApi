using System.Text;
using MySql.Data.MySqlClient;
using ProjectsWebApi.Models;

namespace ProjectsWebApi.Repositories;

public class ProjectRepository : RepositoryBase, IProjectRepository
{
    public ProjectRepository(DatabaseConnectionConfig connectionConfig, TableSchemasConfiguration tableSchemasConfiguration, ILogger<ProjectRepository> logger) : base(connectionConfig, logger)
    {
        InitializeTables(tableSchemasConfiguration.TableSchemas["Projects"]);
    }

    public IEnumerable<Project> FindAll()
    {
        List<Project> projects = new();

        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = "SELECT * FROM projects;";

            connection.Open();
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Project project = GetProjectFromReader(reader);
                    projects.Add(project);
                }
            }
            connection.Close();
        }

        foreach (Project project in projects)
        {
            project.Technologies = FindAllTechnologiesOfProject(project.Id);
        }

        return projects;
    }

    public IEnumerable<Project> FindAllFavorites()
    {
        List<Project> projects = new();

        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = "SELECT * FROM projects WHERE is_favorite=1;";

            connection.Open();
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    Project project = GetProjectFromReader(reader);
                    projects.Add(project);
                }
            }
            connection.Close();
        }

        foreach (Project project in projects)
        {
            project.Technologies = FindAllTechnologiesOfProject(project.Id);
        }

        return projects;
    }

    private string[] FindAllTechnologiesOfProject(Guid projectId)
    {
        List<string> technologies = new();
        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = "SELECT technology_name FROM technologies WHERE technology_id IN (SELECT technology_id FROM project_technologies WHERE project_id=@project_id)";
            command.Parameters.AddWithValue("@project_id", projectId.ToString());
            connection.Open();
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    technologies.Add(reader["technology_name"].ToString() ?? "");
                }
            }
            connection.Close();
        }

        return technologies.ToArray();
    }

    ///<summary>Inserts the Project into the database. Because there is a many-to-many relationship between Projects and Technologies, this requires a few steps.</summary>
    public void Insert(Project project)
    {
        logger.LogInformation("Adding project to database. Project:\n------------------------------\n{project}\n------------------------------", project.ToString());
        InsertProject(project);
        if (project.Technologies.Length > 0)
        {
            InsertTechnologies(project.Technologies);
            Join(project.Id, project.Technologies);
        }
    }

    public void InsertRange(IEnumerable<Project> projects)
    {
        foreach (Project project in projects) Insert(project);
    }
    ///<summary>Adds technologies to the technologies table database if they don't already exist.</summary>
    private void InsertTechnologies(string[] technologies)
    {
        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            StringBuilder sb = new();
            sb.Append("INSERT IGNORE INTO technologies (technology_id, technology_name) VALUES ");

            // Todo: Comment / refactor this code
            List<MySqlParameter> parameters = new();
            for (int i = 0; i < technologies.Length; i++)
            {
                string p1 = "@p1_" + i;
                string p2 = "@p2_" + i;

                sb.Append($"({p1}, {p2}), ");

                MySqlParameter parameter1 = new(p1, Guid.NewGuid().ToString());
                MySqlParameter parameter2 = new(p2, technologies[i]);
                parameters.Add(parameter1);
                parameters.Add(parameter2);
            }
            sb.Length -= 2;
            command.Parameters.AddRange(parameters.ToArray());
            command.CommandText = sb.ToString();
            logger.LogDebug("Executing: {command}", sb.ToString());
            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

    ///<summary>Adds the project to the projects table.</summary>
    private void InsertProject(Project project)
    {
        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = $"INSERT INTO projects (project_id, project_name, short_description, long_description, site, source, is_favorite) values (@id, @name, @shortDesc, @longDesc, @site, @source, @isFavorite)";
            command.Parameters.AddWithValue("@id", project.Id);
            command.Parameters.AddWithValue("@name", project.Name);
            command.Parameters.AddWithValue("@shortDesc", project.ShortDescription);
            command.Parameters.AddWithValue("@longDesc", project.LongDescription);
            command.Parameters.AddWithValue("@site", project.Site);
            command.Parameters.AddWithValue("@source", project.Source);
            command.Parameters.AddWithValue("@isFavorite", project.IsFavorite ? 1 : 0);

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    ///<summary>Joins the technologies and projects tables on the project_technologies table</summary>
    private void Join(Guid id, string[] technologies)
    {
        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            StringBuilder sb = new();
            sb.Append("INSERT INTO project_technologies (project_id, technology_id) SELECT @project_id, technology_id FROM technologies WHERE technology_name IN (");

            // Todo: Comment / refactor this code
            List<MySqlParameter> parameters = new();
            for (int i = 0; i < technologies.Length; i++)
            {
                string p = "@p_"+i;
                sb.Append($"{p}, ");
                MySqlParameter parameter = new(p, technologies[i]);
                parameters.Add(parameter);
            }
            sb.Length -= 2;
            sb.Append(")");

            command.Parameters.AddRange(parameters.ToArray());
            command.Parameters.AddWithValue("@project_id", id.ToString());
            command.CommandText = sb.ToString();

            logger.LogDebug("Executing: {command}", sb.ToString());

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    ///<summary>Unjoins all technologies for the given project id on table project_technologies.</summary>
    private void Unjoin(Guid projectId)
    {
        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = "DELETE FROM project_technologies WHERE project_id = @project_id;";
            command.Parameters.AddWithValue("@project_id", projectId.ToString());
            
            connection.Open();
            logger.LogDebug("Executing: {command}", "DELETE FROM projects WHERE project_id = @project_id;");
            command.ExecuteNonQuery();
            connection.Close();
        }
    }
    ///<summary>Builds a Project instance based on the current row that MySqlDataReader is reading from the database.</summary>
    private Project GetProjectFromReader(MySqlDataReader reader)
    {
        Project project = new()
        {
            Id = Guid.Parse(reader["project_id"].ToString() ?? ""),
            Name = reader["project_name"].ToString() ?? "",
            ShortDescription = reader["short_description"].ToString() ?? "",
            LongDescription = reader["long_description"].ToString() ?? "",
            // Technologies are built with another query later
            Site = reader["site"].ToString() ?? "",
            Source = reader["source"].ToString() ?? "",
            IsFavorite = Boolean.Parse(reader["is_favorite"].ToString())
        };
        return project;
    }
    ///<summary>Deletes from projects and project_technologies the rows with the given Id</summary>
    public void DeleteById(Guid id)
    {
        logger.LogInformation("Removing project from the database, project id: {id}", id);
        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand commandA = new();
            commandA.Connection = connection;
            commandA.CommandText = "DELETE FROM project_technologies WHERE project_id = @project_id;";
            commandA.Parameters.AddWithValue("@project_id", id.ToString());

            MySqlCommand commandB = new();
            commandB.Connection = connection;
            commandB.CommandText = "DELETE FROM projects WHERE project_id = @project_id;";
            commandB.Parameters.AddWithValue("@project_id", id.ToString());
            
            connection.Open();
            
            logger.LogDebug("Executing: {command}", "DELETE FROM project_technologies WHERE project_id = @project_id;");
            commandA.ExecuteNonQuery();
            
            logger.LogDebug("Executing: {command}", "DELETE FROM projects WHERE project_id = @project_id;");
            commandB.ExecuteNonQuery();
            
            connection.Close();
        }
    }

    ///<summary>Updates the project in projects with new values, then reconstructs the relevant rows in project_technologies as well</summary>
    public void Update(Project project)
    {
        logger.LogInformation("Updating project in database. Project:\n------------------------------\n{project}\n------------------------------", project.ToString());
        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = "UPDATE projects SET project_name = @name, short_description = @short, long_description = @long, site = @site, source = @source, is_favorite = @fav WHERE project_id = @id";
            command.Parameters.AddWithValue("@name", project.Name);
            command.Parameters.AddWithValue("@short", project.ShortDescription);
            command.Parameters.AddWithValue("@long", project.LongDescription);
            command.Parameters.AddWithValue("@site", project.Site);
            command.Parameters.AddWithValue("@source", project.Source);
            command.Parameters.AddWithValue("@fav", project.IsFavorite ? 1 : 0);
            command.Parameters.AddWithValue("@id", project.Id.ToString());

            connection.Open();
            logger.LogDebug("Executing: {command}", "UPDATE projects SET project_name = @name, short_description = @short, long_description = @longc, site = @site, source = @source, is_favorite = @fav WHERE project_id = @id");
            command.ExecuteNonQuery();
            connection.Close();

            Unjoin(project.Id);
            Join(project.Id, project.Technologies);
        }
    }
}