using ProjectsWebApi.Repositories;

namespace ProjectsWebApi.Services;

public class PasswordService : IPasswordService
{
  private readonly IPasswordRepository passwordRepository;

  public PasswordService(IPasswordRepository passwordRepository)
  {
    this.passwordRepository = passwordRepository;
    string password = passwordRepository.GetPassword();
    if (password == "")
    {
      ChangePassword(Environment.GetEnvironmentVariable("ASPNETCORE_PASSWORD") ?? "_");
    }
  }

  public void ChangePassword(string newPassword)
  {
    string hash = BCrypt.Net.BCrypt.HashPassword(newPassword);
    passwordRepository.SetPassword(hash);
  }

  public bool IsPasswordCorrect(string password)
  {
    string validPassword = passwordRepository.GetPassword();
    return BCrypt.Net.BCrypt.Verify(password, validPassword);
  }
}