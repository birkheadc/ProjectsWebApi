using Microsoft.AspNetCore.Mvc;
using ProjectsWebApi.Filters;
using ProjectsWebApi.Models;
using ProjectsWebApi.Services;

namespace ProjectsWebApi.Controllers;

[ApiController]
[Route("api/project")]
public class ProjectController : ControllerBase
{

    private readonly IProjectService service;
    private readonly ILogger<ProjectController> logger;

    public ProjectController(IProjectService service, ILogger<ProjectController> logger)
    {
        this.service = service;
        this.logger = logger;
    }

    [HttpGet]
    [Route("debug")]
    public IActionResult Debug()
    {
        return Ok();
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        logger.LogDebug("Request received: GetAll");
        try
        {
            return Ok(service.GetAllProjects());
        }
        catch
        {
            return BadRequest("Something went wrong!");
        }
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
        catch
        {
            return BadRequest("Something went wrong!");
        }
    }

    [HttpPost]
    [Route("debug")]
    [PasswordAuth]
    public IActionResult DEBUG_PopulateDatabase()
    {
        List<ProjectIncoming> projects = new();
        projects.Add(new ProjectIncoming()
        {
            Name = "Game",
            ShortDescription = "My game.",
            LongDescription = "A longer description of my game.",
            Technologies = new string[] {
                "c#",
                "unity"
            },
            Site = "game.birkheadc.me",
            Source = "https://github.com/stars/birkheadc/lists/game",
            IsFavorite = false
        });
        projects.Add(new ProjectIncoming()
        {
            Name = "Bookkeeper",
            ShortDescription = "My bookkeeping app.",
            LongDescription = "The app I use to record my gross sales at my business.",
            Technologies = new string[] {
                "html5",
                "css",
                "javascript"
            },
            Site = "bookkeeper.birkheadc.me",
            Source = "https://github.com/stars/birkheadc/lists/bookkeeperlist",
            IsFavorite = true
        });
        try
        {
            service.AddProjects(projects);
            return Ok();
        }
        catch
        {
            return BadRequest("Something went wrong!");
        }
    }
}