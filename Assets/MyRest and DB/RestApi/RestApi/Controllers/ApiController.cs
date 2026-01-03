using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestApi.Models;
using RestApi.Data;

namespace RestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApiController : ControllerBase
    {
        private readonly API_Context _context;

        public ApiController(API_Context context)
        {
            _context = context;
        }

        //POST
        [HttpPost]
        public async Task<IActionResult> CreateEdit([FromBody] UsersDT user)
        {
            if (string.IsNullOrEmpty(user.Id))
            {
                return BadRequest("User ID cannot be null or empty.");
            }
            
            var userInDb = await _context.users.FindAsync(user.Id);
            if (userInDb == null) _context.users.Add(user);
            
            else 
            {
                //Update
                userInDb.Name = user.Name;
                userInDb.skin_ID = user.skin_ID;
            }
            

            await _context.SaveChangesAsync();
            return Ok(user);
        }

        //GET
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id)
        {
            var result = await _context.users.FindAsync(id);
            if (result == null) return NotFound();
            return Ok(result);
        }

        //DELETE
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _context.users.FindAsync(id);
            if (result == null) return NotFound();

            _context.users.Remove(result);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        //GET ALL
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var result = await _context.users.ToListAsync();
            return Ok(result);
        }
    }
}