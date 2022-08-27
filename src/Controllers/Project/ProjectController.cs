using Microsoft.AspNetCore.Mvc;
using ProjectsWebApi.Models;

namespace ProjectsWebApi.Controllers;

[ApiController]
[Route("api/project")]
public class ProjectController : ControllerBase
{
    public ProjectController()
    {
        
    }

    [HttpGet]
    public IActionResult GetAll()
    {
        try
        {
            return Ok();
        }
        catch
        {
            return BadRequest();
        }
    }
}