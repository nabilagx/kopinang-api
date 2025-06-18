using kopinang_api.Data;
using kopinang_api.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using kopinang_api.Attributes;

namespace kopinang_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [FirebaseAuthorize]
    public class UlasanController : ControllerBase
    {
        private readonly DBContext _context;

        public UlasanController(DBContext context)
        {
            _context = context;
        }
        
        [HttpPost]
        public async Task<IActionResult> CreateUlasan([FromBody] Ulasan ulasan)
        {
            var userId = ulasan.UserId;
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId harus diisi.");
                
            var order = await _context.orders
                .Where(o => o.Id == ulasan.OrderId && o.UserId == userId && o.Status == "Selesai")
                .FirstOrDefaultAsync();

            if (order == null)
                return BadRequest("Order tidak ditemukan atau belum selesai.");

            bool alreadyReviewed = await _context.ulasan
                .AnyAsync(u => u.OrderId == ulasan.OrderId && u.UserId == userId);

            if (alreadyReviewed)
                return BadRequest("Anda sudah memberikan ulasan untuk order ini.");

            ulasan.CreatedAt = DateTime.UtcNow;
            ulasan.UpdatedAt = DateTime.UtcNow;

            _context.ulasan.Add(ulasan);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUlasanById), new { id = ulasan.Id }, ulasan);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUlasanById(int id)
        {
            var ulasan = await _context.ulasan.FindAsync(id);
            if (ulasan == null)
                return NotFound();

            return Ok(ulasan);
        }

        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetUlasanByOrder(int orderId)
        {
            var ulasan = await _context.ulasan
                .Where(u => u.OrderId == orderId)
                .ToListAsync();

            return Ok(ulasan);
        }

        [HttpGet("user/{userId}")]
        public async Task<IActionResult> GetUlasanByUser(string userId)
        {
            if (string.IsNullOrEmpty(userId))
                return BadRequest("UserId harus diisi.");

            var ulasan = await _context.ulasan
                .Where(u => u.UserId == userId)
                .ToListAsync();

            return Ok(ulasan);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> PatchAdminReply(int id, [FromBody] AdminReplyDto dto)
        {
            var ulasan = await _context.ulasan.FindAsync(id);
            if (ulasan == null)
                return NotFound(new { message = "Ulasan tidak ditemukan" });

            if (!string.IsNullOrEmpty(ulasan.AdminReply))
            {
                return BadRequest(new { message = "Balasan admin sudah pernah diberikan dan tidak dapat diubah." });
            }

            ulasan.AdminReply = dto.AdminReply;
            ulasan.UpdatedAt = DateTime.UtcNow;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException ex)
            {
                return StatusCode(500, new { message = "Gagal menyimpan data", detail = ex.Message });
            }

            return Ok(new { message = "Balasan admin berhasil diupdate" });
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<Ulasan>>> GetAll()
        {
            var ulasanList = await _context.ulasan.ToListAsync();
            return Ok(ulasanList);
        }
    }
}
