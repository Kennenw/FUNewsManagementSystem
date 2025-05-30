using FUNewsManagementSystem.WebMVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Web;

namespace FUNewsManagementSystem.WebMVC.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7069/odata/");
        }

        public async Task<IActionResult> Index(string searchQuery = "", string sortOrder = "desc", int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var token = Request.Cookies["Token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "Auth");
                }

                var query = "Categories?$count=true";

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var encodedQuery = HttpUtility.UrlEncode(searchQuery);
                    query += $"&$filter=contains(tolower(CategoryName),'{encodedQuery.ToLower()}')";
                }

                var skip = (pageNumber - 1) * pageSize;
                query += $"&$top={pageSize}&$skip={skip}";

                var response = await _httpClient.GetAsync(query);
                var rawJson = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    ViewBag.Error = $"API request failed with status {response.StatusCode}: {rawJson}";
                    return View(new ListViewModel<CategoryViewModel>());
                }

                // Deserialize with case-insensitive option
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var catogory = await response.Content.ReadFromJsonAsync<ODataResponse<CategoryViewModel>>(options);

                var viewModel = new ListViewModel<CategoryViewModel>
                {
                    Item = catogory.Value ?? new List<CategoryViewModel>(),
                    SearchQuery = searchQuery,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = catogory?.Count ?? 0
                };

                return View(viewModel);
            }
            catch (JsonException ex)
            {
                var response = await _httpClient.GetAsync("NewsArticle");
                var rawJson = await response.Content.ReadAsStringAsync();
                ViewBag.Error = $"JSON deserialization failed: {ex.Message}. Raw response: {rawJson}";
                return View(new ListViewModel<CategoryViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Không thể tải dữ liệu từ API. Lỗi {ex.Message}";
                return View(new ListViewModel<CategoryViewModel>());
            }
        }
    }
}
