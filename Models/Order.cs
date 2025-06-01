using System;
using System.Collections.Generic;

namespace kopinang_api.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string UserId { get; set; }        // Firestore UID customer
        public int TotalHarga { get; set; }
        public string Status { get; set; }
        public string MetodePembayaran { get; set; }
        public string BuktiPembayaran { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<OrderDetail> OrderDetails { get; set; }
    }
}
