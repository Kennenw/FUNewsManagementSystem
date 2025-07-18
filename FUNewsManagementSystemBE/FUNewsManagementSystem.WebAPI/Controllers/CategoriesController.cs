﻿
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.AspNetCore.OData.Routing.Attributes;
using Microsoft.AspNetCore.Authorization;
using FUNewsManagementSystem.Reposirories.ViewModels;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [Route("odata/Categories")]
    [ApiController]
    public class CategoriesController : ODataController
    {
        private readonly ICategoryService _service;

        public CategoriesController(ICategoryService service)
        {
            _service = service;
        }

        [EnableQuery]
        [HttpGet]
        public ActionResult<IQueryable<Category>> Get()
        {
            var newsCategory = _service.GetAllAsync();
            return Ok(newsCategory);
        }

        // GET: api/Categories/5
        [EnableQuery]
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(short id)
        {
            var category = await _service.GetByIdAsync(id);
            if (category != null)
            {
                return Ok(category);
            }
            return NotFound();
        }

        // PUT: api/Categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> Put(short id, [FromBody] UpdateCategoryViewModels category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.UpdateAsync(id, category);
            return Updated(category);
        }

        // POST: api/Categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> Post([FromBody]CreateCategoryViewModels category)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = await _service.AddAsync(category);
            if (item == null)
                return BadRequest("Unable to create category");
            return Created(item);
        }

        // DELETE: api/Categories/5
        [HttpDelete("{id}")]
        [Authorize(Policy = "Staff")]
        public async Task<IActionResult> Delete(short id)
        {
            var hasNews = await _service.CheckCategoryAsync(id);
            if (hasNews)
            {
                return BadRequest("Cannot delete category: it has existing news articles or not found category.");
            }
            await _service.DeleteAsync(id);
            return Ok("Account deleted successfully.");
        }
    }
}
