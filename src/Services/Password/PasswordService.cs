using ProjectsWebApi.Repositories;

namespace ProjectsWebApi.Services;

public class PasswordService : IPasswordService
{
  private readonly IPasswordRepository passwordRepository;

  public PasswordService(IPasswordRepository passwordRepository)
  {
    this.passwordRepository = passwordRepository;
  }

  public void ChangePassword(string newPassword)
  {
    throw new NotImplementedException();
  }

  public bool IsPasswordCorrect(string password)
  {
    string validPassword = passwordRepository.GetPassword();
    return BCrypt.Net.BCrypt.Verify(password, validPassword);
  }
}