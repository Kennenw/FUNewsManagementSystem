using FUNewsManagementSystem.WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace FUNewsManagementSystem.WebMVC.Controllers
{
    [Authorize(Policy = "Staff")]
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7069/odata/");
        }

        public async Task<IActionResult> Index(string searchQuery = "", string sortBy = "CategoryId", string sortOrder = "asc", int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var token = Request.Cookies["Token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var query = "Categories?$count=true";

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var encodedQuery = HttpUtility.UrlEncode(searchQuery);
                    query += $"&$filter=contains(tolower(CategoryName),'{encodedQuery.ToLower()}')";
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    var validSortBy = sortBy switch
                    {
                        "CategoryId" => "CategoryId",
                        "CategoryName" => "CategoryName",
                        _ => "CategoryId"
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
                    return View(new ListViewModel<CategoryViewModel>());
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var category = await response.Content.ReadFromJsonAsync<ODataResponse<CategoryViewModel>>(options);

                var viewModel = new ListViewModel<CategoryViewModel>
                {
                    Item = category?.Value ?? new List<CategoryViewModel>(),
                    SearchQuery = searchQuery,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = category?.Count ?? 0
                };

                ViewBag.AllCategories = viewModel.Item ?? new List<CategoryViewModel>();

                return View(viewModel);
            }
            catch (JsonException ex)
            {
                ViewBag.Error = $"JSON deserialization failed: {ex.Message}.";
                return View(new ListViewModel<CategoryViewModel>());
            }
            catch (Exception ex)
            {
                ViewBag.Error = $"Không thể tải dữ liệu từ API. Lỗi: {ex.Message}";
                return View(new ListViewModel<CategoryViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }

            await LoadCategoriesAsync();
            return View(new CreateCategoryViewModels { IsActive = true });
        }

        // POST: Create a new category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModels model)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(model);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Prepare payload to match API schema exactly (note: API has typo "CategoryDesciption")
            var payload = new
            {
                categoryName = model.CategoryName?.Trim(),
                categoryDesciption = model.CategoryDescription?.Trim() ?? "", // API expects this field with typo
                parentCategoryId = model.ParentCategoryId.HasValue && model.ParentCategoryId.Value > 0 ? model.ParentCategoryId.Value : (int?)null,
                isActive = true
            };

            // Debug logging
            var payloadJson = System.Text.Json.JsonSerializer.Serialize(payload);
            Console.WriteLine($"Sending payload: {payloadJson}");

            var response = await _httpClient.PostAsJsonAsync("Categories", payload);
            var rawJson = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"Response status: {response.StatusCode}");
            Console.WriteLine($"Response body: {rawJson}");

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Lỗi khi tạo danh mục: {rawJson}";
                await LoadCategoriesAsync();
                return View(model);
            }

            return RedirectToAction("Index");
        }

        // GET: Show edit form for a category
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"Categories/{id}");
            var rawJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Không thể tải danh mục: {rawJson}";
                return RedirectToAction("Index");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var category = await response.Content.ReadFromJsonAsync<CategoryViewModel>(options);

            if (category == null)
            {
                TempData["Error"] = "Danh mục không tồn tại.";
                return RedirectToAction("Index");
            }
            UpdateCategoryViewModels updateCategory = new UpdateCategoryViewModels()
            {
                CategoryId = category.CategoryId,
                CategoryDescription = category.CategoryDescription,
                CategoryName = category.CategoryName,
                IsActive = category.IsActive ?? true,
                ParentCategoryId = category.ParentCategoryId,
            };

            await LoadCategoriesAsync();
            return View(updateCategory);
        }

        // POST: Update an existing category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateCategoryViewModels model)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }

            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync();
                return View(model);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            // Prepare payload to match API schema exactly (note: API has typo "CategoryDesciption")
            var payload = new
            {
                categoryId = model.CategoryId,
                categoryName = model.CategoryName?.Trim(),
                categoryDesciption = model.CategoryDescription?.Trim() ?? "", // API expects this field with typo
                parentCategoryId = model.ParentCategoryId.HasValue && model.ParentCategoryId.Value > 0 ? model.ParentCategoryId.Value : (int?)null,
                isActive = true
            };

            var response = await _httpClient.PutAsJsonAsync($"Categories/{id}", payload);
            var rawJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Cập nhật thất bại: {rawJson}";
                await LoadCategoriesAsync();
                return View(model);
            }

            return RedirectToAction("Index");
        }

        // POST: Delete a category
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var token = Request.Cookies["Token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                // Use the same format as other controllers (NewsArticles, SystemAccounts)
                Console.WriteLine($"Attempting to delete category with ID: {id}");
                var response = await _httpClient.DeleteAsync($"Categories/{id}");

                if (response.IsSuccessStatusCode)
                {
                    TempData["Success"] = "Category deleted successfully!";
                    return RedirectToAction("Index");
                }

                var errorContent = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Delete API Error: {response.StatusCode} - {errorContent}");
                TempData["Error"] = $"Failed to delete category: {errorContent}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Delete Exception: {ex.Message}");
                TempData["Error"] = $"Error deleting category: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        // Load categories for dropdown
        private async Task LoadCategoriesAsync()
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
            {
                ViewBag.Categories = new List<SelectListItem>();
                return;
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.GetAsync("Categories");
            var json = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Categories = new List<SelectListItem>();
                return;
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var odataResponse = JsonSerializer.Deserialize<ODataResponse<CategoryViewModel>>(json, options);

            var categories = odataResponse?.Value?.Select(c => new SelectListItem
            {
                Value = c.CategoryId.ToString(),
                Text = c.CategoryName
            }).ToList() ?? new List<SelectListItem>();

            ViewBag.Categories = categories;
        }

    }
}