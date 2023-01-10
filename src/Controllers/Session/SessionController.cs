using Microsoft.AspNetCore.Mvc;
using ProjectsWebApi.Filters;
using ProjectsWebApi.Models;
using ProjectsWebApi.Services;

namespace ProjectsWebApi.Controllers;

[ApiController]
[Route("api/session")]
public class SessionController: ControllerBase
{

  private readonly ISessionService sessionService;

  public SessionController(ISessionService sessionService)
  {
    this.sessionService = sessionService;
  }

  [HttpGet]
  [PasswordAuth]
  public IActionResult GetSessionToken()
  {
    try
    {
      SessionToken token = sessionService.GenerateToken();
      return Ok(token);
    }
    catch
    {
      return StatusCode(9001);
    }
  }

  [HttpPost]
  [TokenAuth]
  public IActionResult ValidateSessionToken()
  {
    return Ok();
  }
}