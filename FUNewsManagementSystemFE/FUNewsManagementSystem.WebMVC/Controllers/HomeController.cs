using FUNewsManagementSystem.WebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
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

        public async Task<IActionResult> Index(string searchQuery = "", string sortBy = "CreatedDate", string sortOrder = "desc", int pageNumber = 1, int pageSize = 3)
        {
            try
            {
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
                    return View(new NewsArticleListViewModel());
                }

                // Deserialize with case-insensitive option
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var articles = await response.Content.ReadFromJsonAsync<ODataResponse<NewsArticleViewModel>>(options);

                var viewModel = new NewsArticleListViewModel
                {
                    Articles = articles.Value ?? new List<NewsArticleViewModel>(),
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
                return View(new NewsArticleListViewModel());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Không thể tải dữ liệu từ API. Lỗi {ex.Message}";
                return View(new NewsArticleListViewModel());
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
