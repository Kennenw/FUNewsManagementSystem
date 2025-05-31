using FUNewsManagementSystem.WebMVC.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace FUNewsManagementSystem.WebMVC.Controllers
{
    [Authorize(Policy = "Admin")]
    public class ReportController : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public ReportController(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
        }

        public IActionResult Index()
        {
            return View(new ReportViewModel
            {
                StartDate = DateTime.Today.AddYears(-10),
                EndDate = DateTime.Today
            });
        }

        [HttpPost]
        public async Task<IActionResult> Index(ReportViewModel model)
        {
            var minDate = DateTime.Today.AddYears(-10);
            if (!ModelState.IsValid ||
                model.StartDate < minDate ||
                model.StartDate > (model.EndDate == default ? DateTime.Today : model.EndDate) ||
                model.EndDate > DateTime.Today)
            {
                if (model.StartDate < minDate)
                    ModelState.AddModelError("StartDate", $"Start Date cannot be earlier than {minDate:yyyy-MM-dd}.");
                if (model.StartDate > (model.EndDate == default ? DateTime.Today : model.EndDate))
                    ModelState.AddModelError("", "Start Date must be before or equal to End Date.");
                if (model.EndDate > DateTime.Today)
                    ModelState.AddModelError("EndDate", $"End Date cannot be later than today ({DateTime.Today:yyyy-MM-dd}).");
                return View(model);
            }

            try
            {
                using var client = _httpClientFactory.CreateClient();
                client.DefaultRequestHeaders.Add("Authorization", $"Bearer {Request.Cookies["Token"]}");
                var query = $"?startDate={model.StartDate:yyyy/M/d}";
                if (model.EndDate != default)
                {
                    query += $"&endDate={model.EndDate:yyyy/M/d}";
                }
                var response = await client.GetAsync($"https://localhost:7069/api/Report{query}");

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var report = JsonSerializer.Deserialize<ReportViewModel>(content, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    return View(report);
                }

                ModelState.AddModelError("", $"Failed to generate report: {response.ReasonPhrase}");
                return View(model);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Error generating report: {ex.Message}");
                return View(model);
            }
        }
    }
}