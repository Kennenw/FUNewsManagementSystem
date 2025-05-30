using FUNewsManagementSystem.WebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Web;

namespace FUNewsManagementSystem.WebMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7069/odata/");
        }


        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Index", "Auth");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"NewsArticles/{id}");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể tải bài viết.";
                return RedirectToAction("Index");
            }

            var article = await response.Content.ReadFromJsonAsync<UpdateNewsArticleViewModels>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            await LoadCategoriesAsync();
            return View(article);
        }


        [HttpPost]
        public async Task<IActionResult> Edit(string id, UpdateNewsArticleViewModels model)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token)) return RedirectToAction("Index", "Auth");

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(model);
            }

            // Tự động gán UpdatedById và ModifiedDate
            model.UpdatedById = GetUserIdFromToken(token); // Hoặc lấy từ token nếu bạn giải mã JWT
            model.ModifiedDate = DateTime.Now;

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJsonAsync($"NewsArticles/{id}", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Cập nhật thất bại.";
            await LoadCategoriesAsync();
            return View(model);
        }




        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
                return Unauthorized();

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"NewsArticles/{id}");

            return response.IsSuccessStatusCode ? Ok() : StatusCode((int)response.StatusCode);
        }




        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Auth");

            await LoadCategoriesAsync();

            var viewModel = new CreateNewsArticleViewModels
            {
                CreatedById = GetUserIdFromToken(token)
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateNewsArticleViewModels model)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Auth");

            model.CreatedById = GetUserIdFromToken(token);

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(model);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("NewsArticles", model);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var error = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Lỗi khi tạo bài viết: {error}";


            //ViewBag.Error = "Lỗi khi tạo bài viết: " + errorMessage;
            await LoadCategoriesAsync();
            return View(model);
        }

        private async Task LoadCategoriesAsync()
        {
            var response = await _httpClient.GetAsync("Categories");
            var json = await response.Content.ReadAsStringAsync();
            var doc = JsonDocument.Parse(json);

            var categories = doc.RootElement.GetProperty("value")
                .EnumerateArray()
                .Select(c => new SelectListItem
                {
                    Value = c.GetProperty("CategoryId").GetInt16().ToString(),
                    Text = c.GetProperty("CategoryName").GetString()
                }).ToList();

            ViewBag.Categories = categories;
        }

        private short GetUserIdFromToken(string token)
        {
            var handler = new JwtSecurityTokenHandler();

            if (!handler.CanReadToken(token))
                throw new ArgumentException("Token không hợp lệ");

            var jwtToken = handler.ReadJwtToken(token);
            var accountIdClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "AccountId");

            if (accountIdClaim == null)
                throw new InvalidOperationException("Token không chứa AccountId");

            if (short.TryParse(accountIdClaim.Value, out short accountId))
            {
                return accountId;
            }
            else
            {
                throw new InvalidOperationException("AccountId trong token không phải số hợp lệ");
            }
        }




        public async Task<IActionResult> Index(string searchQuery = "", string sortBy = "CreatedDate", string sortOrder = "desc", int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var token = Request.Cookies["Token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "Auth");
                }

                var query = "NewsArticles?$count=true";
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var encodedQuery = HttpUtility.UrlEncode(searchQuery);
                    query += $"&$filter=contains(tolower(NewsTitle),'{encodedQuery.ToLower()}') or contains(tolower(NewsContent),'{encodedQuery.ToLower()}')";
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    var validSortBy = sortBy switch
                    {
                        "NewsTitle" => "NewsTitle",
                        "CreatedDate" => "CreatedDate",
                        _ => "NewsArticleId"
                    };
                    var validSortOrder = sortOrder?.ToLower() == "asc" ? "asc" : "desc";
                    query += $"&$orderby={validSortBy} {validSortOrder}";
                }

                var skip = (pageNumber - 1) * pageSize;
                query += $"&$top={pageSize}&$skip={skip}";

                var response = await _httpClient.GetAsync(query);
                var rawJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"API request failed with status {response.StatusCode}: {rawJson}";
                    return View(new ListViewModel<NewsArticleViewModel>());
                }

                // Deserialize with case-insensitive option
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var articles = await response.Content.ReadFromJsonAsync<ODataResponse<NewsArticleViewModel>>(options);

                var viewModel = new ListViewModel<NewsArticleViewModel>
                {
                    Item = articles.Value ?? new List<NewsArticleViewModel>(),
                    SearchQuery = searchQuery,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = articles?.Count ?? 0
                };

                return View(viewModel);
            }
            catch (JsonException ex)
            {
                var response = await _httpClient.GetAsync("NewsArticle");
                var rawJson = await response.Content.ReadAsStringAsync();
                ViewBag.Error = $"JSON deserialization failed: {ex.Message}. Raw response: {rawJson}";
                return View(new ListViewModel<NewsArticleViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Không thể tải dữ liệu từ API. Lỗi {ex.Message}";
                return View(new ListViewModel<NewsArticleViewModel>());
            }
        }


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
