using FUNewsManagementSystem.WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text.Json;
using System.Web;

namespace FUNewsManagementSystem.WebMVC.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;

        public HomeController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7069/odata/");
        }


        [HttpGet]
        public async Task<IActionResult> Details(string id)
        {
            try
            {
                var token = Request.Cookies["Token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"NewsArticles/{id}?$expand=Category,Tags,CreatedBy");

                if (!response.IsSuccessStatusCode)
                {
                    TempData["Error"] = "Unable to load article. Please try again later.";
                    return RedirectToAction("Index");
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var article = await response.Content.ReadFromJsonAsync<NewsDetailViewModel>(options);

                if (article == null)
                {
                    TempData["Error"] = "Article not found.";
                    return RedirectToAction("Index");
                }

                var viewModel = new NewsDetailViewModel
                {
                    NewsArticleId = article.NewsArticleId,
                    NewsTitle = article.NewsTitle,
                    Headline = article.Headline,
                    NewsContent = article.NewsContent,
                    NewsSource = article.NewsSource,
                    CategoryId = article.CategoryId,
                    Category = article.Category, 
                    NewsStatus = article.NewsStatus,
                    CreatedById = article.CreatedById,
                    CreatedBy = article.CreatedBy,
                    CreatedDate = article.CreatedDate,
                    ModifiedDate = article.ModifiedDate,
                    Tags = article.Tags ?? new List<TagsViewModels>()
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error loading article: {ex.Message}";
                return RedirectToAction("Index");
            }
        }


        [Authorize(Policy = "Staff")]
        [HttpGet]
        public async Task<IActionResult> Edit(string id)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token)) 
                return RedirectToAction("Index", "Auth");

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync($"NewsArticles/{id}?$expand=Category,Tags,CreatedBy");

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = "Không thể tải bài viết.";
                return RedirectToAction("Index");
            }

            var article = await response.Content.ReadFromJsonAsync<UpdateNewsArticleViewModels>(
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            var viewModel = new UpdateNewsArticleViewModels
            {
                NewsArticleId = article.NewsArticleId,
                NewsTitle = article.NewsTitle,
                Headline = article.Headline,
                NewsContent = article.NewsContent,
                NewsSource = article.NewsSource,
                CategoryId = article.CategoryId,
                NewsStatus = article.NewsStatus,
                UpdatedById = GetUserIdFromToken(token),
                SelectedTagIds = article.Tags?.Select(t => t.TagId).ToList() ?? new List<int>(),
                ModifiedDate = DateTime.UtcNow
            };
            await LoadCategoriesAsync();
            return View(viewModel);
        }

        [Authorize(Policy = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UpdateNewsArticleViewModels model)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token)) 
                return RedirectToAction("Index", "Auth");

            if (!ModelState.IsValid || id != model.NewsArticleId)
            {
                await LoadCategoriesAsync();
                return View(model);
            }

            model.UpdatedById = GetUserIdFromToken(token);
            var payload = new
            {
                newsTitle = model.NewsTitle,
                headline = model.Headline,
                newsContent = model.NewsContent,
                newsSource = model.NewsSource,
                categoryId = model.CategoryId,
                newsStatus = true,
                updatedById = model.UpdatedById,
                tags = model.SelectedTagIds,
                modifiedDate = model.ModifiedDate ?? DateTime.UtcNow 
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.PutAsJsonAsync($"NewsArticles/{id}", payload);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            ViewBag.Error = "Cập nhật thất bại.";
            await LoadCategoriesAsync();
            return View(model);
        }



        [Authorize(Policy = "Staff")]
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



        [Authorize(Policy = "Staff")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
                return RedirectToAction("Index", "Auth");

            await LoadCategoriesAsync();

            var viewModel = new CreateNewsArticleViewModels
            {
                CreatedById = GetUserIdFromToken(token),
                NewsStatus = true
            };

            return View(viewModel);
        }

        [Authorize(Policy = "Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
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
            var payload = new
            {
                model.NewsTitle,
                model.Headline,
                CreatedDate = DateTime.Now,
                model.NewsContent,
                model.NewsSource,
                model.CategoryId,
                NewsStatus = true,
                tagIds = model.SelectedTagIds,
                model.CreatedById
            };

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("NewsArticles", payload);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            var error = await response.Content.ReadAsStringAsync();
            ViewBag.Error = $"Lỗi khi tạo bài viết: {error}";
            await LoadCategoriesAsync();
            return View(model);
        }

        private async Task LoadCategoriesAsync()
        {
            var token = Request.Cookies["Token"];
            if (!string.IsNullOrEmpty(token))
            {
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            var catResponse = await _httpClient.GetAsync("Categories?$filter=IsActive eq true");
            var tagResponse = await _httpClient.GetAsync("Tags");
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            var categories = await catResponse.Content.ReadFromJsonAsync<ODataResponse<CategoryViewModel>>(options);
            var tags = await tagResponse.Content.ReadFromJsonAsync<ODataResponse<TagsViewModels>>(options);

            ViewBag.Categories = categories?.Value.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList() ?? new List<SelectListItem>();

            ViewBag.Tags = tags?.Value.Select(t => new SelectListItem
            {
                Value = t.TagId.ToString(),
                Text = t.TagName
            }).ToList() ?? new List<SelectListItem>();
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

                var query = "NewsArticles?$count=true&$expand=Category,Tags,CreatedBy";
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
                Console.WriteLine($"Ds: {rawJson}");
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
                    Item = articles?.Value.Select(a => new NewsArticleViewModel
                    {
                        NewsArticleId = a.NewsArticleId,
                        NewsTitle = a.NewsTitle,
                        Headline = a.Headline,
                        CreatedDate = a.CreatedDate,
                        NewsContent = a.NewsContent,
                        NewsSource = a.NewsSource,
                        CategoryId = a.CategoryId,
                        NewsStatus = a.NewsStatus,
                        CreatedById = a.CreatedById,
                        UpdatedById = a.UpdatedById,
                        ModifiedDate = a.ModifiedDate,
                        CategoryName = a.Category?.CategoryName,
                        CreatedByName = a.CreatedBy?.AccountName,
                        Tags = a.Tags ?? new List<TagsViewModels>()
                    }).ToList() ?? new List<NewsArticleViewModel>(),
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

        [Authorize(Policy = "Staff")]
        public IActionResult History()
        {
            return View(new ArticleHistoryViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> History(ArticleHistoryViewModel model)
        {
            if (!ModelState.IsValid || model.AccountId <= 0)
            {
                ModelState.AddModelError("AccountId", "Please enter a valid Account ID.");
                return View(model);
            }

            try
            {
                var token = Request.Cookies["Token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
                var response = await _httpClient.GetAsync($"NewsArticles/history?accountId={model.AccountId}");
                var rawJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"API request failed with status {response.StatusCode}: {rawJson}";
                    return View(new ArticleHistoryViewModel());
                }
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                var articles = JsonSerializer.Deserialize<List<NewsArticleHistoryViewModels>>(rawJson, options);

                model.Articles = articles?.Select(a => new NewsArticleHistoryViewModels
                {
                    NewsArticleId = a.NewsArticleId,
                    NewsTitle = a.NewsTitle,
                    Headline = a.Headline,
                    CategoryName = a.CategoryName,
                    AuthorName = a.AuthorName,
                    CreatedDate = a.CreatedDate,
                    TagName = a.TagName ?? new List<string>(),
                    NewsStatus = a.NewsStatus
                }).ToList() ?? new List<NewsArticleHistoryViewModels>();

                return View(model);
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"JSON deserialization failed: {ex.Message}";
            }

            return View(model);
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
