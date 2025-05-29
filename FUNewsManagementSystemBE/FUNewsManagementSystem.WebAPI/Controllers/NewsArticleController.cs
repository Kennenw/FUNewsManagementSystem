using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
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
        public ActionResult<IQueryable<NewsArticle>> Get()
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

        [HttpPost]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> Post([FromBody] CreateNewsArticleViewModels newsArticle)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.AddAsync(newsArticle);
            return Created(newsArticle);
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
