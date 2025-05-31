using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Reposirories.ViewModels;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FUNewsManagementSystem.WebAPI.Controllers
{
    [ApiController]
    [Route("odata/SystemAccounts")]
    public class SystemAccountController : ODataController
    {
        private readonly ISystemAccountService _service;

        public SystemAccountController(ISystemAccountService service)
        {
            _service = service;
        }

        [EnableQuery]
        [HttpGet]
        [Authorize(Policy = "Admin")]
        public ActionResult<IQueryable<SystemAccount>> Get()
        {
            var accounts = _service.GetAllAsync();
            return Ok(accounts);
        }

        [EnableQuery]
        [HttpGet("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Get(short id)
        {
            var account = await _service.GetByIdAsync(id);
            if (account != null)
            {
                return Ok(account);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Post([FromBody] CreateSystemAccountViewModel account)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            var item = await _service.AddAsync(account);
            return Created(item);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Put(short id, [FromBody] UpdateSystemAccountViewModel account)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.UpdateAsync(id,account);
            return Updated(account);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "Admin")]
        public async Task<IActionResult> Delete(short id)
        {
            var hasNews = await _service.CheckUserAsync(id);
            if (hasNews)
            {
                return BadRequest("Cannot delete account: it has existing news articles or not found user.");
            }
            await _service.DeleteAsync(id);
            return Ok("Account deleted successfully.");
        }
    }
}
