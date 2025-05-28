using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

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
        [Authorize(Policy = "Staff")]
        public ActionResult<IQueryable<NewsArticle>> Get()
        {
            var newsArticle = _service.GetAllAsync();
            return Ok(newsArticle);
        }

        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> Get(string key)
        {
            var newsArticle = await _service.GetByIdAsync(key);
            if (newsArticle != null)
            {
                return Ok(newsArticle);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] NewsArticle newsArticle)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.AddAsync(newsArticle);
            return Created(newsArticle);
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Put(string key, [FromBody] NewsArticle newsArticle)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            newsArticle.NewsArticleId = key;
            await _service.UpdateAsync(newsArticle);
            return Updated(newsArticle);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(string key)
        {
            await _service.DeleteAsync(key);
            return NoContent();
        }
    }
}
