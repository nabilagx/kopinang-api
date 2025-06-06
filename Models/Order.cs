using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace kopinang_api.Models
{
    [Table("orders")]
    public class Order
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("user_id")]
        public string UserId { get; set; }        // Firestore UID customer

        [Column("total_harga")]
        public int TotalHarga { get; set; }

        [Column("status")]
        public string Status { get; set; }

        [Column("metode_pembayaran")]
        public string MetodePembayaran { get; set; }

        [Column("bukti_pembayaran")]
        public string BuktiPembayaran { get; set; }

        [Column("latitude")]
        public double? Latitude { get; set; }

        [Column("longitude")]
        public double? Longitude { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
        [Column("qrcode")]
        public string? QrCode { get; set; }


        // Relasi dengan order_detail
        [InverseProperty("Order")]
        public List<OrderDetail> OrderDetails { get; set; }
    }
}
