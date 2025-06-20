﻿using kopinang_api.Data;
using kopinang_api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using kopinang_api.Attributes;

namespace kopinang_api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [FirebaseAuthorize]
    public class ProdukController : ControllerBase
    {
        private readonly DBContext _context;

        public ProdukController(DBContext context)
        {
            _context = context;
        }
        [HttpGet]
         public async Task<ActionResult<IEnumerable<Produk>>> GetProduk()
         {
             try
             {
                 return await _context.produk.ToListAsync();
             }
             catch (Exception ex)
             {
                 return StatusCode(500, $"Internal Server Error: {ex.Message}");
             }
         }


        [HttpGet("{id}")]
        public async Task<ActionResult<Produk>> GetProduk(int id)
        {
            var produk = await _context.produk.FindAsync(id);
            if (produk == null) return NotFound();
            return produk;
        }

        [HttpPost]
        public async Task<ActionResult<Produk>> CreateProduk(Produk produk)
        {
            produk.CreatedAt = DateTime.UtcNow;
            produk.UpdatedAt = DateTime.UtcNow;

            _context.produk.Add(produk);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetProduk), new { id = produk.Id }, produk);
            
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduk(int id, Produk updated)
        {
            var produk = await _context.produk.FindAsync(id);
            if (produk == null) return NotFound();

            // Update field yang diizinkan
            produk.Nama = updated.Nama;
            produk.Deskripsi = updated.Deskripsi;
            produk.Harga = updated.Harga;
            produk.Stok = updated.Stok;
            produk.Gambar = updated.Gambar;
            produk.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new { message = "Produk berhasil diupdate" });
        }

        [HttpPut("{id}/kurangi-stok")]
        public async Task<IActionResult> KurangiStok(int id, [FromBody] KurangiStokDto dto)
        {
            var produk = await _context.produk.FindAsync(id);
            if (produk == null)
                return NotFound(new { message = "Produk tidak ditemukan." });

            if (produk.Stok < dto.Jumlah)
                return BadRequest(new { message = "Stok tidak mencukupi." });

            produk.Stok -= dto.Jumlah;
            produk.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Stok berhasil dikurangi.",
                produk.Id,
                produk.Nama,
                produk.Stok
            });
        }
        
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteProduk(int id)
        {
            var produk = await _context.produk.FindAsync(id);
            if (produk == null)
                return NotFound(new { message = "Produk tidak ditemukan" });

            _context.produk.Remove(produk);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Produk berhasil dihapus" });
        }

    }
}
