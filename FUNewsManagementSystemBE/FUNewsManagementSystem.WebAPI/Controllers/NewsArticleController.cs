using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.EntityFrameworkCore;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("odata/NewsArticles")]
    [ApiController]
    public class NewsArticleController : ODataController
    {
        private readonly INewsArticleService _service;

        public NewsArticleController(INewsArticleService service)
        {
            _service = service;
        }

        [EnableQuery]
        [HttpGet]
        public IActionResult Get()
        {
            var newsArticle = _service.GetAllAsync();
            return Ok(newsArticle);
        }

        [EnableQuery]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            var newsArticle = await _service.GetByIdAsync(id);
            if (newsArticle != null)
            {
                return Ok(newsArticle);
            }
            return NotFound();
        }

        [HttpGet("history")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> GetArticleHistory([FromQuery] short accountId)
        {
            if (accountId <= 0)
            {
                return BadRequest(new { message = "Invalid account ID." });
            }
            try
            {
                var articles = await _service.GetNewsModify(accountId);

                if (!articles.Any())
                {
                    return NotFound(new { message = "No articles found for the specified account." });
                }
                return Ok(articles);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Error retrieving article history: {ex.Message}" });
            }
        }

        [HttpPost]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> Post([FromBody] CreateNewsArticleViewModels newsArticle)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.AddAsync(newsArticle);
            return Ok(newsArticle);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> Put(string id, [FromBody] UpdateNewsArticleViewModels newsArticle)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.UpdateAsync(id,newsArticle);
            return Updated(newsArticle);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> Delete(string id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}
