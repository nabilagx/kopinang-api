using kopinang_api.Data;
using kopinang_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using kopinang_api.Attributes;



namespace kopinang_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [FirebaseAuthorize]
    public class OrderController : ControllerBase
    {
        private readonly DBContext _context;

        public OrderController(DBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Order>>> GetAllOrders()
        {
            var orders = await _context.orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Produk)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            return Ok(orders);
        }


        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder(Order order)
        {
            order.Status = "Diproses";
            order.CreatedAt = System.DateTime.UtcNow;
            order.UpdatedAt = System.DateTime.UtcNow;

            _context.orders.Add(order);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetOrder), new { id = order.Id }, order);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(int id)
        {
            var order = await _context.orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Produk)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return NotFound();

            return order;
        }
        [HttpGet("user/{uid}")]
        public async Task<ActionResult<IEnumerable<Order>>> GetOrdersByUserId(string uid)
        {
            var orders = await _context.orders
                .Where(o => o.UserId == uid)
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Produk)
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();

            if (orders == null || !orders.Any())
            {
                return NotFound();
            }

            return orders;
        }
        [HttpPut("{id}/qrcode")]
        public async Task<IActionResult> UpdateQrCode(int id, [FromBody] dynamic data)
        {
            var order = await _context.orders.FindAsync(id);
            if (order == null)
            {
                return NotFound();
            }

            try
            {
                string qrCode = data.qrCode; // Pastikan penulisan sama: qrCode
                order.QrCode = qrCode;
                order.UpdatedAt = DateTime.UtcNow;

                await _context.SaveChangesAsync();
                return Ok(order);
            }
            catch (Exception ex)
            {
                return StatusCode(500, ex.ToString());
            }
        }
        [HttpPut("{id}/status")]
        public async Task<IActionResult> UpdateStatus(int id, [FromBody] string newStatus)
        {
            var order = await _context.orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Pesanan tidak ditemukan" });

            if (order.Status.ToLower() == "selesai")
                return BadRequest(new { message = "Pesanan sudah selesai dan tidak bisa diubah" });

            order.Status = newStatus;
            order.UpdatedAt = DateTime.UtcNow;



            await _context.SaveChangesAsync();
            return Ok(new { message = "Status pesanan diperbarui" });
        }
        [HttpPut("{id}/verifikasi")]
        public async Task<IActionResult> VerifikasiPesanan(int id)
        {
            var order = await _context.orders.FindAsync(id);
            if (order == null)
                return NotFound(new { message = "Pesanan tidak ditemukan" });

            if (order.Status.ToLower() == "selesai")
                return BadRequest(new { message = "Pesanan sudah selesai dan tidak bisa diverifikasi lagi" });

            order.Status = "Selesai";  // langsung paksa selesai
            order.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return Ok(new { message = "Pesanan berhasil diverifikasi dan status diubah jadi Selesai" });
        }


        [HttpPost("payment/qris")]
        public async Task<IActionResult> CreateQrisTransaction([FromBody] MidtransQrisRequest request)
        {
            if (request.totalHarga <= 0)
            {
                return BadRequest(new { message = "TotalHarga must be greater than 0." });
            }

            var client = new HttpClient();
            var serverKey = Environment.GetEnvironmentVariable("MIDTRANS_SERVER_KEY")
                ?? "fallback-key-untuk-dev-opsional";

            var base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(serverKey + ":"));

            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);

            var midtransRequest = new
            {
                payment_type = "qris",
                transaction_details = new
                {
                    order_id = request.orderId,
                    gross_amount = request.totalHarga
                },
                customer_details = new
                {
                    first_name = request.nama,
                    email = request.email
                }
            };

            var json = JsonConvert.SerializeObject(midtransRequest);
            Console.WriteLine("Payload Midtrans: " + json);  // Logging

            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.sandbox.midtrans.com/v2/charge", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            Console.WriteLine("Response Midtrans: " + responseBody);  // Logging

            return Content(responseBody, "application/json");
        }


        [HttpPost("{orderId}/verifikasi-midtrans")]
        public async Task<IActionResult> VerifikasiMidtrans(string orderId)
        {
            var client = new HttpClient();
            var serverKey = Environment.GetEnvironmentVariable("MIDTRANS_SERVER_KEY")
                ?? "fallback-key";  // atau ambil dari appsettings
        
            var base64Auth = Convert.ToBase64String(Encoding.UTF8.GetBytes(serverKey + ":"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", base64Auth);
        
            var response = await client.GetAsync($"https://api.sandbox.midtrans.com/v2/{orderId}/status");
            var body = await response.Content.ReadAsStringAsync();
        
            dynamic result = JsonConvert.DeserializeObject(body);
            string transactionStatus = result.transaction_status;
        
            if (transactionStatus != "settlement")
                return BadRequest(new { message = $"Status pembayaran: {transactionStatus}" });
        
            if (!int.TryParse(orderId, out var id))
                return BadRequest("Invalid Order ID");
        
            var order = await _context.orders.FirstOrDefaultAsync(o => o.Id == id);
            if (order == null) return NotFound();
        
            order.Status = "Diproses";
            order.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        
            return Ok(new { message = "Pembayaran berhasil diverifikasi dan status pesanan diperbarui" });
        }
    }
}
