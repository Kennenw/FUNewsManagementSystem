using FUNewsManagementSystem.Reposirories.ViewModels;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : ControllerBase
    {
        private readonly ISystemAccountService _service;

        public AuthorController(ISystemAccountService service)
        {
            _service = service;
        }

        [HttpPost("Login")]
        public async Task<ActionResult> Login([FromBody] LoginRequestViewModel model)
        {
            try
            {
                var account = await _service.LoginAsync(model);
                if (account == null)
                {
                    return Unauthorized("Invalid email or password.");
                }

                IConfiguration configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, true).Build();

                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Email, account.AccountEmail.ToString() ?? "admin@123.gmail.com"),
                    new Claim("AccountName", account.AccountName ?? "Admin"),
                    new Claim("Role", account.AccountRole.ToString()),
                    new Claim("AccountId", account.AccountId.ToString()),
                };

                var symetricKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JWT:SecretKey"]));
                var signCredential = new SigningCredentials(symetricKey, SecurityAlgorithms.HmacSha256);

                var preparedToken = new JwtSecurityToken(
                    issuer: configuration["JWT:Issuer"],
                    audience: configuration["JWT:Audience"],
                    claims: claims,
                    expires: DateTime.Now.AddMinutes(16),
                    signingCredentials: signCredential);

                var generatedToken = new JwtSecurityTokenHandler().WriteToken(preparedToken);
                var role = account.AccountRole.ToString();
                var accountId = account.AccountId.ToString();

                return Ok(new LoginResponseViewModel
                {
                    Role = role,
                    Token = generatedToken,
                    AccountId = accountId
                });
            }
            catch (Exception e)
            {
                return BadRequest(e.Message);
            }
        }
    }
}
