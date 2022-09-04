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

    public ProjectController(IProjectService service, ILogger<ProjectController> logger)
    {
        this.service = service;
        this.logger = logger;
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

    [HttpGet]
    [Route("ex")]
    public IActionResult GetException()
    {
        try
        {
            throw new Exception("Whomp whomp");
            return Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Encountered exception. On purpose.");
            return BadRequest("Whomp whomp");
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
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception when attempting to process request InsertProject. Project to be inserted:\n{project}", project.ToString());
            return BadRequest("Something went wrong!");
        }
    }
}