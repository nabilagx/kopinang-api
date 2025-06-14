using Microsoft.AspNetCore.Mvc;
using FirebaseAdmin.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IConfiguration _config;

    public AuthController(IConfiguration config)
    {
        _config = config;
    }

    [HttpPost("firebase-login")]
    public async Task<IActionResult> FirebaseLogin([FromBody] TokenRequest request)
    {
        try
        {
            var decodedToken = await FirebaseAuth.DefaultInstance.VerifyIdTokenAsync(request.IdToken);
            string uid = decodedToken.Uid;

            var jwt = GenerateJwt(uid);
            return Ok(new { token = jwt });
        }
        catch (Exception ex)
        {
            return Unauthorized(new { message = "Invalid Firebase token", error = ex.Message });
        }
    }

    private string GenerateJwt(string uid)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, uid)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}

public class TokenRequest
{
    public string IdToken { get; set; }
}
