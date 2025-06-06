using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace kopinang_api.Models
{
    [Table("order_detail")]
    public class OrderDetail
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("order_id")]
        public int OrderId { get; set; }

        [JsonIgnore]
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }  // nullable supaya aman

        [Column("produk_id")]
        public int ProdukId { get; set; }

        [JsonIgnore]
        [ForeignKey("ProdukId")]
        public Produk? Produk { get; set; } // nullable

        [Column("jumlah")]
        public int Jumlah { get; set; }

        [Column("harga_satuan")]
        public int HargaSatuan { get; set; }
    }
}
