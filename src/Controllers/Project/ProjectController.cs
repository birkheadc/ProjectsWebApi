using Microsoft.AspNetCore.Mvc;
using ProjectsWebApi.Models;
using ProjectsWebApi.Services;

namespace ProjectsWebApi.Controllers;

[ApiController]
[Route("api/project")]
public class ProjectController : ControllerBase
{

    private readonly IProjectService service;

    public ProjectController(IProjectService service)
    {
        this.service = service;
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            return Ok(service.GetAllProjects());
        }
        catch
        {
            return BadRequest("Something went wrong!");
        }
    }
}