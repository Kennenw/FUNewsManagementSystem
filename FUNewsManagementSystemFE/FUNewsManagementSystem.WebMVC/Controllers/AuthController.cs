using FUNewsManagementSystem.WebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;

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
                        Response.Cookies.Append("Token", loginResponse.Token, new CookieOptions
                        {
                            HttpOnly = true, // Ngăn JavaScript truy cập
                            Secure = true,   // Chỉ gửi qua HTTPS
                            SameSite = SameSiteMode.Strict, // Ngăn CSRF
                            Expires = DateTimeOffset.UtcNow.AddDays(1) // Thời hạn cookie
                        });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Login failed: Token is null.");
                        return View(model);
                    }


                    Response.Cookies.Append("Role", loginResponse.Role ?? "");
                    Response.Cookies.Append("AccountId", loginResponse.AccountId ?? "");
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Email hoặc mật khẩu không đúng.");
                return View(model);
            }
        }

        [HttpPost]
        public IActionResult Logout()
        {
            Response.Cookies.Delete("Token");
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
