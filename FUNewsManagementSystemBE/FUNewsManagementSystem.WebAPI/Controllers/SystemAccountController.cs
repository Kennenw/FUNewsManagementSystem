using FUNewsManagementSystem.Reposirories.Models;
using FUNewsManagementSystem.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;

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
        public ActionResult<IQueryable<SystemAccount>> Get()
        {
            var accounts = _service.GetAllAsync();
            return Ok(accounts);
        }

        [EnableQuery]
        [HttpGet("{key}")]
        public async Task<IActionResult> Get(short key)
        {
            var account = await _service.GetByIdAsync(key);
            if (account != null)
            {
                return Ok(account);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            await _service.AddAsync(account);
            return Created(account);
        }

        [HttpPut("{key}")]
        public async Task<IActionResult> Put(short key, [FromBody] SystemAccount account)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);
            account.AccountId = key;
            await _service.UpdateAsync(account);
            return Updated(account);
        }

        [HttpDelete("{key}")]
        public async Task<IActionResult> Delete(short key)
        {
            await _service.DeleteAsync(key);
            return NoContent();
        }
    }
}
