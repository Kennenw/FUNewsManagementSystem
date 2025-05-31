using FUNewsManagementSystem.WebMVC.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.IdentityModel.Tokens.Jwt;

namespace FUNewsManagementSystem.WebMVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;
        public AuthController(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            var token = Request.Cookies["Token"];
            if (!string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Home");
            }
            return View(new LoginRequestViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginRequestViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            using (var client = _httpClientFactory.CreateClient())
            {
                var content = new StringContent(JsonSerializer.Serialize(model), Encoding.UTF8, "application/json");
                var response = await client.PostAsync("https://localhost:7069/api/Author/Login", content);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var options = new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    };

                    var loginResponse = JsonSerializer.Deserialize<LoginResponseViewModel>(result, options);
                    if (!string.IsNullOrEmpty(loginResponse?.Token))
                    {
                        var userName = GetUserNameFromToken(loginResponse.Token);
                        Response.Cookies.Append("Token", loginResponse.Token, new CookieOptions
                        {
                            HttpOnly = true, // Ngăn JavaScript truy cập
                            Secure = true,   // Chỉ gửi qua HTTPS
                            SameSite = SameSiteMode.Strict, // Ngăn CSRF
                            Expires = DateTimeOffset.UtcNow.AddDays(1) // Thời hạn cookie
                        });

                        var claims = new List<Claim>
                        {
                            new Claim(ClaimTypes.Name , userName),
                            new Claim("Role", loginResponse.Role ?? "2"),
                            new Claim("AccountId", loginResponse.AccountId ?? "")
                        };

                        var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                        var authProperties = new AuthenticationProperties
                        {
                            IsPersistent = true,
                            ExpiresUtc = DateTimeOffset.UtcNow.AddDays(1)
                        };

                        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                            new ClaimsPrincipal(claimsIdentity), authProperties);

                        Response.Cookies.Append("Role", loginResponse.Role ?? "");
                        Response.Cookies.Append("AccountId", loginResponse.AccountId ?? "");
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Login failed: Token is null.");
                        return View(model);
                    }
                }

                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> LogoutAsync()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            Response.Cookies.Delete("Token");
            Response.Cookies.Delete("Role");
            Response.Cookies.Delete("AccountId");
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }

        public IActionResult AccessDenied()
        {
            return View();
        }

        private string GetUserNameFromToken(string token)
        {
            try
            {
                var handler = new JwtSecurityTokenHandler();
                if (!handler.CanReadToken(token))
                {
                    throw new ArgumentException("Token không hợp lệ");
                }

                var jwtToken = handler.ReadJwtToken(token);
                var accountClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "AccountName");

                if (accountClaim == null)
                {
                    throw new InvalidOperationException("Token không chứa AccountName");
                }

                if (string.IsNullOrEmpty(accountClaim.Value))
                {
                    throw new InvalidOperationException("AccountName trong token rỗng hoặc không hợp lệ");
                }

                return accountClaim.Value;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Lỗi khi lấy AccountName từ token: {ex.Message}", ex);
            }
        }
    }
}
