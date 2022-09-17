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

    public IEnumerable<Project> FindAll(bool favoriteOnly = false)
    {
        List<Project> projects = new();

        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = favoriteOnly ? "SELECT * FROM projects WHERE is_favorite = 1" : "SELECT * FROM projects;";

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
            project.ShortDescriptions = FindAllShortDescriptionsOfProject(project.Id);
            project.LongDescriptions = FindAllLongDescriptionsOfProject(project.Id);
        }

        return projects;
    }

    public IEnumerable<Project> FindAllFavorites()
    {
        return FindAll(true);
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

    private Description[] FindAllShortDescriptionsOfProject(Guid projectId)
    {
        List<Description> descriptions = new();

        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = "SELECT * FROM short_descriptions WHERE project_id = @pid";
            command.Parameters.AddWithValue("@pid", projectId.ToString());
            
            connection.Open();
            
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                 while (reader.Read())
                 {
                    string language = reader["short_description_language"].ToString() ?? "";
                    string content = reader["short_description_content"].ToString() ?? "";
                    descriptions.Add(new()
                    {
                        Language = language,
                        Content = content
                    });
                 }
            }
            
            connection.Close();
        }

        return descriptions.ToArray();
    }

    private Description[] FindAllLongDescriptionsOfProject(Guid projectId)
    {
        List<Description> descriptions = new();

        using (MySqlConnection connection = GetConnection())
        {
            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = "SELECT * FROM long_descriptions WHERE project_id = @pid";
            command.Parameters.AddWithValue("@pid", projectId.ToString());
            
            connection.Open();
            
            using (MySqlDataReader reader = command.ExecuteReader())
            {
                 while (reader.Read())
                 {
                    string language = reader["long_description_language"].ToString() ?? "";
                    string content = reader["long_description_content"].ToString() ?? "";
                    descriptions.Add(new()
                    {
                        Language = language,
                        Content = content
                    });
                 }
            }
            
            connection.Close();
        }

        return descriptions.ToArray();
    }

    ///<summary>Inserts the Project into the database. Because there is a many-to-many relationship between Projects and Technologies, this requires a few steps.</summary>
    public void Insert(Project project)
    {
        logger.LogInformation("Adding project to database. Project:\n------------------------------\n{project}\n------------------------------", project.ToString());
        InsertProject(project);
        InsertShortDescriptions(project.ShortDescriptions, project.Id);
        InsertLongDescriptions(project.LongDescriptions, project.Id);
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
    ///<summary>Adds the description to the database, language name and description content can then be retreived via the project_id</summary>
    private void InsertShortDescriptions(Description[] descriptions, Guid projectId)
    {
        MySqlCommand command = new();

        StringBuilder sb = new();
        sb.Append("INSERT INTO short_descriptions (short_description_language, short_description_content, project_id) VALUES ");
        foreach (Description description in descriptions)
        {
            string language = description.Language;
            string content = description.Content;

            Dictionary<string, string> parameters = new();
            parameters.Add($"@sdl_{language}", language);
            parameters.Add($"@sdc_{language}", content);
            parameters.Add($"@pid_{language}", projectId.ToString());
            
            sb.Append("(");
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                sb.Append(parameter.Key);
                sb.Append(", ");
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
            sb.Length -= 2;
            sb.Append("), ");
        }
        sb.Length -= 2;
        command.CommandText = sb.ToString();
        using (MySqlConnection connection = GetConnection())
        {
            command.Connection = connection;
            connection.Open();
            logger.LogDebug($"Executing: {command.CommandText}");
            command.ExecuteNonQuery();

            connection.Close();
        }
    }
    ///<summary>Adds the description to the database, language name and description content can then be retreived via the project_id</summary>
    private void InsertLongDescriptions(Description[] descriptions, Guid projectId)
    {
        MySqlCommand command = new();

        StringBuilder sb = new();
        sb.Append("INSERT INTO long_descriptions (long_description_language, long_description_content, project_id) VALUES ");
        foreach (Description description in descriptions)
        {
            string language = description.Language;
            string content = description.Content;

            Dictionary<string, string> parameters = new();
            parameters.Add($"@ldl_{language}", language);
            parameters.Add($"@ldc_{language}", content);
            parameters.Add($"@pid_{language}", projectId.ToString());
            
            sb.Append("(");
            foreach (KeyValuePair<string, string> parameter in parameters)
            {
                sb.Append(parameter.Key);
                sb.Append(", ");
                command.Parameters.AddWithValue(parameter.Key, parameter.Value);
            }
            sb.Length -= 2;
            sb.Append("), ");
        }
        sb.Length -= 2;
        command.CommandText = sb.ToString();
        using (MySqlConnection connection = GetConnection())
        {
            command.Connection = connection;
            connection.Open();
            logger.LogDebug($"Executing: {command.CommandText}");
            command.ExecuteNonQuery();

            connection.Close();
        }
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
            command.CommandText = $"INSERT INTO projects (project_id, project_name, site, source, is_favorite) values (@id, @name, @site, @source, @isFavorite)";
            command.Parameters.AddWithValue("@id", project.Id);
            command.Parameters.AddWithValue("@name", project.Name);
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
            // Descriptions and Technologies are built later
            Site = reader["site"].ToString() ?? "",
            Source = reader["source"].ToString() ?? "",
            IsFavorite = Boolean.Parse(reader["is_favorite"].ToString())
        };
        return project;
    }
    ///<summary>Deletes from projects the rows with the given Id. ON DELETE CASCADE should be enabled in relevant tables, so there is no need to manually delete from linked tables.</summary>
    public void DeleteById(Guid id)
    {
        logger.LogInformation("Removing project from the database, project id: {id}", id);
        using (MySqlConnection connection = GetConnection())
        {

            MySqlCommand command = new();
            command.Connection = connection;
            command.CommandText = "DELETE FROM projects WHERE project_id = @project_id;";
            command.Parameters.AddWithValue("@project_id", id.ToString());
            
            connection.Open();
            
            logger.LogDebug("Executing: {command}", "DELETE FROM projects WHERE project_id = @project_id;");
            command.ExecuteNonQuery();
            
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
            command.CommandText = "UPDATE projects SET project_name = @name, site = @site, source = @source, is_favorite = @fav WHERE project_id = @id";
            command.Parameters.AddWithValue("@name", project.Name);
            command.Parameters.AddWithValue("@site", project.Site);
            command.Parameters.AddWithValue("@source", project.Source);
            command.Parameters.AddWithValue("@fav", project.IsFavorite ? 1 : 0);
            command.Parameters.AddWithValue("@id", project.Id.ToString());

            connection.Open();
            logger.LogDebug("Executing: {command}", command.CommandText);
            command.ExecuteNonQuery();
            connection.Close();

            Unjoin(project.Id);
            Join(project.Id, project.Technologies);

            UpdateShortDescriptions(project.ShortDescriptions, project.Id);
            UpdateLongDescriptions(project.LongDescriptions, project.Id);
        }
    }
    ///<summary>Inserts rows for each language if they do not exist, then updates the contents of those rows to match new description. If the description is empty, remove that description instead.</summary>
    private void UpdateShortDescriptions(Description[] descriptions, Guid projectId)
    {
        using (MySqlConnection connection = GetConnection())
        {
            foreach (Description description in descriptions)
            {
                MySqlCommand command = new();
                command.Connection = connection;
                if (description.Language == "")
                {
                    command.CommandText = "DELETE FROM short_descriptions WHERE short_description_language = @sdl";
                }
                else
                {
                    command.CommandText = "INSERT INTO short_descriptions (short_description_language, short_description_content, project_id) VALUES (@sdl, @sdc, @pid) ON DUPLICATE KEY UPDATE short_description_content = @sdc";
                }
                command.Parameters.AddWithValue("@sdl", description.Language);
                command.Parameters.AddWithValue("@sdc", description.Content);
                command.Parameters.AddWithValue("@pid", projectId.ToString());

                connection.Open();
                
                command.ExecuteNonQuery();
                
                connection.Close();
            }
        }
    }
    
    ///<summary>Inserts rows for each language if they do not exist, then updates the contents of those rows to match new description. If the description is empty, remove that description instead.</summary>
    private void UpdateLongDescriptions(Description[] descriptions, Guid projectId)
    {
        using (MySqlConnection connection = GetConnection())
        {
            foreach (Description description in descriptions)
            {
                MySqlCommand command = new();
                command.Connection = connection;
                if (description.Language == "")
                {
                    command.CommandText = "DELETE FROM long_descriptions WHERE long_description_language = @ldl";
                }
                else
                {
                    command.CommandText = "INSERT INTO long_descriptions (long_description_language, long_description_content, project_id) VALUES (@ldl, @ldc, @pid) ON DUPLICATE KEY UPDATE long_description_content = @ldc";
                }
                command.Parameters.AddWithValue("@ldl", description.Language);
                command.Parameters.AddWithValue("@ldc", description.Content);
                command.Parameters.AddWithValue("@pid", projectId.ToString());

                connection.Open();
                
                command.ExecuteNonQuery();
                
                connection.Close();
            }
        }
    }
}