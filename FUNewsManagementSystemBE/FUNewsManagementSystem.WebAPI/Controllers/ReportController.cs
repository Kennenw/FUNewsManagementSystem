using FUNewsManagementSystem.Reposirories.ViewModels;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] 
    public class ReportController : ControllerBase
    {
        private readonly IReportService _service;

        public ReportController(IReportService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<IActionResult> GetReport([FromQuery] DateTime startDate, [FromQuery] DateTime? endDate)
        {
            try
            {
                var actualEndDate = endDate ?? DateTime.Now;

                var roleClaim = User.FindFirst("Role")?.Value;
                if (roleClaim != "3")
                {
                    return Forbid("Only Admins can access this report.");
                }

                if (startDate > endDate)
                {
                    return BadRequest(new { Message = "StartDate must be before EndDate." });
                }

                var report = await _service.GetNewsReport(startDate, actualEndDate);

                return Ok(report);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = $"Error generating report: {ex.Message}" });
            }
        }
    }
}
