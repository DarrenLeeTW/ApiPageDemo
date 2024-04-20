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

        [HttpPost("InsertFriends")]
        public async Task<IActionResult> InsertFriends(List<Friend> friends)
        {
            if (friends == null || !friends.Any())
            {
                return BadRequest("No friends provided.");
            }

            try
            {
                await _context.Friends.AddRangeAsync(friends);
                await _context.SaveChangesAsync();
                return Ok($"Successfully inserted {friends.Count} friends.");
            }
            catch (Exception ex)
            {
                // Log exception details here to understand what went wrong
                return StatusCode(500, "Internal Server Error: " + ex.Message);
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetFriends([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 5)
        {
            try
            {
                // 取得總好友數
                var totalFriends = await _context.Friends.CountAsync();

                // 將總好友數除以每頁大小（pageSize)，取得總頁數
                var totalPages = (int)System.Math.Ceiling(totalFriends / (double)pageSize);

                // 確認頁碼有效
                if (pageNumber < 1 || pageNumber > totalPages)
                {
                    return BadRequest("Invalid page number.");
                }

                // 計算當前頁面的好友
                // Skip:跳過前(pageNumber -1) *pageSize 條記錄，以達到定位到正確的分頁開始位置
                // Take:從當前位置取 pageSize 條記錄，即取一頁的數據量
                var friends = await _context.Friends
                                    .OrderBy(f => f.Id)
                                    .Skip((pageNumber - 1) * pageSize)
                                    .Take(pageSize)
                                    .ToListAsync();

                // 建立回應
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

