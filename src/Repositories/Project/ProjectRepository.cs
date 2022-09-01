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
        Console.WriteLine("Attempting to insert project...");

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
            Console.WriteLine($"Executing: {sb.ToString()}");
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
            command.CommandText = $"INSERT INTO projects (project_id, project_name, short_description, long_description, site, source) values (@id, @name, @shortDesc, @longDesc, @site, @source)";
            command.Parameters.AddWithValue("@id", project.Id);
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
                Console.WriteLine($"{p} == {technologies[i]}");
                parameters.Add(parameter);
            }
            sb.Length -= 2;
            sb.Append(")");

            command.Parameters.AddRange(parameters.ToArray());
            command.Parameters.AddWithValue("@project_id", id.ToString());
            Console.WriteLine($"@project_id == {id.ToString()}");
            command.CommandText = sb.ToString();

            Console.WriteLine($"Executing code: {sb.ToString()}");

            connection.Open();
            command.ExecuteNonQuery();
            connection.Close();
        }
    }

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
            Source = reader["source"].ToString() ?? ""
        };
        return project;
    }
}