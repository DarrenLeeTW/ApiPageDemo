using ApiPageDemo.Models;
using Microsoft.AspNetCore.Mvc;
using ApiPageDemo.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Azure;


namespace ApiPageDemo.Controllers
{
   
    [Route("api/[controller]")]
    [ApiController]
    public class FriendsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FriendsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetFriends([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                var totalFriends = await _context.Friends.CountAsync();
                var totalPages = (int)System.Math.Ceiling(totalFriends / (double)pageSize);

                if (pageNumber < 1 || pageNumber > totalPages)
                {
                    return BadRequest("Invalid page number.");
                }

                var friends = await _context.Friends
                                    .OrderBy(f => f.Name)
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

                var response = new
                {
                    TotalPages = totalPages,
                    CurrentPage = pageNumber,
                    PageSize = pageSize,
                    TotalFriends = totalFriends,
                    Friends = friends
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }
    }
}

