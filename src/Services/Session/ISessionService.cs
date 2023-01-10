using ProjectsWebApi.Models;

namespace ProjectsWebApi.Services;

public interface ISessionService
{
  public bool IsTokenValid(string token);
  public SessionToken GenerateToken();
}