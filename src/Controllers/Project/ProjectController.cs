using Microsoft.AspNetCore.Mvc;
using ProjectsWebApi.Filters;
using ProjectsWebApi.Models;
using ProjectsWebApi.Services;

namespace ProjectsWebApi.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController : ControllerBase
{
    private readonly IProjectService service;
    private readonly ILogger<ProjectController> logger;
    private readonly IWebHostEnvironment env;

    public ProjectController(IProjectService service, ILogger<ProjectController> logger, IWebHostEnvironment env)
    {
        this.service = service;
        this.logger = logger;
        this.env = env;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            return Ok(service.GetAllProjects());
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception when attempting to process request GetAll.");
            return BadRequest("Something went wrong!");
        }
    }

    [HttpGet]
    [Route("favorites")]
    public IActionResult GetFavorites()
    {
        try
        {
            return Ok(service.GetAllFavoriteProjects());
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception when attempting to process request GetFavorites.");
            return BadRequest("Something went wrong!");
        }
    }

    [HttpPost]
    [Route("populate")]
    [PasswordAuth]
    public IActionResult DEBUG_Populate()
    {
        if (env.IsDevelopment() == false)
        {
            return NotFound();
        }
        try
        {
            List<ProjectIncoming> projects = new();
            for (int i = 0; i < 10; i++)
            {
                ProjectIncoming project = new()
                {
                    Name = $"Project_{i}",
                    ShortDescriptions = GetShortDescriptions(),
                    LongDescriptions = GetLongDescriptions(),
                    Technologies = RandomTechs(),
                    Site = $"www.project.{i}.com",
                    Source = $"git.project.{i}.com"
                };
                projects.Add(project);
            }
            service.AddProjects(projects);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception when attempting to process request Populate.");
            return BadRequest("Something went wrong!");
        }
    }

    private Description[] GetShortDescriptions()
    {
        Description[] d = new Description[2];
        d[0] = new Description
        {
            Language = "en",
            Content = "stuff"
        };
        d[1] = new Description
        {
            Language = "bb",
            Content = "Gobbledygook"
        }; 
        return d;
    }

    private Description[] GetLongDescriptions()
    {
        Description[] d = new Description[2];
        d[0] = new Description
        {
            Language = "en",
            Content = "A slightly longer description, with more words and stuff."
        };
        d[1] = new Description
        {
            Language = "bb",
            Content = "Gobbledygook Gobbledygook Gobbledygook Gobbledygook"
        }; 
        return d;
    }

    private string[] RandomTechs()
    {
        string[] techs = new string[]
        {
            "c#",
            "html5",
            "javascript",
            "css",
            "java",
            "spring",
            "unity",
            "python",
            "c",
            "c++",
            "unreal_engine",
            "aspnet",
            "lua",
            "react"
        };
        Random a = new Random();
        Random b = new Random();

        int num = a.Next(1, 4);

        string[] s = new string[num];
        
        for (int i = 0; i < num; i++)
        {
            int j = b.Next(0, 13);
            s[i] = techs[j];
        }
        return s;
    }

    [HttpPost]
    [PasswordAuth]
    public IActionResult InsertProject(ProjectIncoming project)
    {
        try
        {
            service.AddProject(project);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception when attempting to process request InsertProject. Project to be inserted:\n{project}", project.ToString());
            return BadRequest("Something went wrong!");
        }
    }

    [HttpDelete]
    [PasswordAuth]
    [Route("{id}")]
    public IActionResult DeleteProject([FromRoute (Name = "id")] string id)
    {
        Guid guid;
        try
        {
            guid = Guid.Parse(id);
        }
        catch
        {
            return BadRequest($"Id [{id}] is not valid format.");
        }
        try
        {
            service.DeleteProjectById(guid);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception when attempting to process request DeleteProject. Project Id to be deleted: {id}", id);
            return BadRequest("Something went wrong!");
        }
    }

    [HttpPut]
    [PasswordAuth]
    public IActionResult UpdateProject(Project project)
    {
        try
        {
            service.UpdateProject(project);
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception when attempting to process request UpdateProject. Project to be updated:\n{project}", project.ToString());
            return BadRequest("Something went wrong!");
        }
    }
}