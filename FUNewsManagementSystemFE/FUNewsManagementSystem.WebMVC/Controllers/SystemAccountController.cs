using FUNewsManagementSystem.WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Web;

namespace FUNewsManagementSystem.WebMVC.Controllers
{
    [Authorize(Policy = "Admin")]
    public class SystemAccountController : Controller
    {
        private readonly HttpClient _httpClient;

        public SystemAccountController(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("https://localhost:7069/odata/");
        }

        public async Task<IActionResult> Index(string searchQuery = "", string sortBy = "AccountId", string sortOrder = "asc", int pageNumber = 1, int pageSize = 3)
        {
            try
            {
                var token = Request.Cookies["Token"];
                if (string.IsNullOrEmpty(token))
                {
                    return RedirectToAction("Index", "Auth");
                }

                _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                var query = "SystemAccounts?$count=true";

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    var encodedQuery = HttpUtility.UrlEncode(searchQuery);
                    query += $"&$filter=contains(tolower(AccountName),'{encodedQuery.ToLower()}') or contains(tolower(AccountEmail),'{encodedQuery.ToLower()}')";
                }

                if (!string.IsNullOrEmpty(sortBy))
                {
                    var validSortBy = sortBy switch
                    {
                        "AccountId" => "AccountId",
                        "AccountName" => "AccountName",
                        _ => "AccountId"
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
                    return View(new ListViewModel<SystemAccountViewModel>());
                }

                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var category = await response.Content.ReadFromJsonAsync<ODataResponse<SystemAccountViewModel>>(options);

                var viewModel = new ListViewModel<SystemAccountViewModel>
                {
                    Item = category?.Value ?? new List<SystemAccountViewModel>(),
                    SearchQuery = searchQuery,
                    SortBy = sortBy,
                    SortOrder = sortOrder,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalRecords = category?.Count ?? 0
                };

                ViewBag.AllCategories = category?.Value ?? new List<SystemAccountViewModel>();

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
        public IActionResult Create()
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }

            ViewBag.Roles = new List<SelectListItem>
        {
            new SelectListItem { Value = "3", Text = "Admin" },
            new SelectListItem { Value = "2", Text = "Lecturer" },
            new SelectListItem { Value = "1", Text = "Staff" }
        };

            return View(new CreateSystemAccountViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSystemAccountViewModel model)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "3", Text = "Admin" },
                new SelectListItem { Value = "2", Text = "Lecturer" },
                new SelectListItem { Value = "1", Text = "Staff" }
            };
                return View(model);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.PostAsJsonAsync("SystemAccounts", model);
            var rawJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Lỗi khi tạo tài khoản: {rawJson}";
                ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "3", Text = "Admin" },
                new SelectListItem { Value = "2", Text = "Lecturer" },
                new SelectListItem { Value = "1", Text = "Staff" }
            };
                return View(model);
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await _httpClient.GetAsync($"SystemAccounts/{id}");
            var rawJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                TempData["Error"] = $"Không thể tải tài khoản: {rawJson}";
                return RedirectToAction("Index");
            }

            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var account = await response.Content.ReadFromJsonAsync<SystemAccountViewModel>(options);

            if (account == null)
            {
                TempData["Error"] = "Tài khoản không tồn tại.";
                return RedirectToAction("Index");
            }

            ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "3", Text = "Admin", Selected = account.AccountRole == 3 },
                new SelectListItem { Value = "2", Text = "Lecturer", Selected = account.AccountRole == 2 },
                new SelectListItem { Value = "1", Text = "Staff", Selected = account.AccountRole == 1 }
            };
            UpdateSystemAccountViewModel model = new UpdateSystemAccountViewModel
            {
                AccountEmail = account.AccountEmail,
                AccountName = account.AccountName,
                AccountPassword = account.AccountPassword,
                AccountRole = account.AccountRole
            };

            return View(model);
        }

        // POST: Update an existing account
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateSystemAccountViewModel model)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("Index", "Auth");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "3", Text = "Admin" },
                new SelectListItem { Value = "2", Text = "Lecturer" },
                new SelectListItem { Value = "1", Text = "Staff" }
            };
                return View(model);
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            

            var response = await _httpClient.PutAsJsonAsync($"SystemAccounts/{id}", model);
            var rawJson = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                ViewBag.Error = $"Cập nhật thất bại: {rawJson}";
                ViewBag.Roles = new List<SelectListItem>
            {
                new SelectListItem { Value = "3", Text = "Admin" },
                new SelectListItem { Value = "2", Text = "Lecturer" },
                new SelectListItem { Value = "1", Text = "Staff" }
            };
                return View(model);
            }

            return RedirectToAction("Index");
        }



        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var token = Request.Cookies["Token"];
            if (string.IsNullOrEmpty(token))
            {
                return Unauthorized();
            }

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            var response = await _httpClient.DeleteAsync($"SystemAccounts/{id}");

            if (response.IsSuccessStatusCode)
            {
                return Ok();
            }

            return StatusCode((int)response.StatusCode);
        }


    }
}
