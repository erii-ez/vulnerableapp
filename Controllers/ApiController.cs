using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http; 
using System.Linq; 
using VulnerableApp.Data;

namespace VulnerableApp.Controllers
{
    [ApiController]
    [Route("api")]
    public class ApiController : ControllerBase
    {
        private readonly AppDbContext _db;
        
        public ApiController(AppDbContext db) 
        { 
            _db = db; 
        }

        [HttpGet("user/{id}")]
        public IActionResult GetUser(int id)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!currentUserId.HasValue) return Unauthorized();
            
            if (id != currentUserId.Value) return Forbid();
            
            var user = _db.Users.Find(id);
            if (user == null) return NotFound();
            return Ok(new 
            { 
                user.Id, 
                user.Username, 
                user.Email 
            });
        }

        [HttpGet("users")]
        public IActionResult GetAllUsers()
        {
            
            var currentUserId = HttpContext.Session.GetInt32("UserId");
            if (!currentUserId.HasValue) return Unauthorized();

            var safeUsers = _db.Users.Select(u => new
            {
                u.Id,
                u.Username,
                u.Email
            }).ToList();

            return Ok(safeUsers);
        }
    }
}