using ProjectsWebApi.Models;

namespace ProjectsWebApi.Services;

public class SessionService : ISessionService
{
  public SessionToken GenerateToken()
  {
    // Todo
    return new SessionToken()
    {
      Token = "token"
    };
  }

  public bool IsTokenValid(string token)
  {
    // Todo
    return token == "token";
  }
}