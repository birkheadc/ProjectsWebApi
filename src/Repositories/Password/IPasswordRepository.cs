namespace ProjectsWebApi.Repositories;

public interface IPasswordRepository
{
  public string GetPassword();
  public void SetPassword(string password);
}