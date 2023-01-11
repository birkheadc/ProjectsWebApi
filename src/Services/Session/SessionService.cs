using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ProjectsWebApi.Config;
using ProjectsWebApi.Models;

namespace ProjectsWebApi.Services;

public class SessionService : ISessionService
{
  private readonly JwtConfig jwtConfig;
  private readonly JwtSecurityTokenHandler tokenHandler;

  public SessionService(JwtConfig jwtConfig)
  {
    this.jwtConfig = jwtConfig;
    tokenHandler = new();
  }
  
  public SessionToken GenerateToken()
  {
    SecurityToken token = tokenHandler.CreateToken(GetSecurityTokenDescriptor());
    return new SessionToken()
    {
      Token = tokenHandler.WriteToken(token)
    };
  }
  
  private SecurityTokenDescriptor GetSecurityTokenDescriptor()
  {
    return new SecurityTokenDescriptor()
    {
      Expires = DateTime.UtcNow.AddDays(1),
      SigningCredentials = new
      (
        new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Key ?? "")),
        SecurityAlgorithms.HmacSha512Signature
      )
    };
  }

  public bool IsTokenValid(string token)
  {
    try
    {
      tokenHandler.ValidateToken(
        token,
        GetTokenValidationParameters(),
        out SecurityToken _
      );
      return true;
    }
    catch
    {
      return false;
    }
  }

  private TokenValidationParameters GetTokenValidationParameters()
  {
    return new TokenValidationParameters()
    {
      ValidateIssuerSigningKey = true,
      IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtConfig.Key ?? "")),
      ValidateIssuer = false,
      ValidateAudience = false,
      ClockSkew = TimeSpan.Zero
    };
  }
}