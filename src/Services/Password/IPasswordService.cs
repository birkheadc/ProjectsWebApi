namespace ProjectsWebApi.Services;

public interface IPasswordService
{
  public void ChangePassword(string newPassword);
  public bool IsPasswordCorrect(string password);
}