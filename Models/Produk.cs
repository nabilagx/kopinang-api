using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace kopinang_api.Models
{
    [Table("produk")]
    public class Produk
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("nama")]
        public string Nama { get; set; }

        [Column("deskripsi")]
        public string Deskripsi { get; set; }

        [Column("gambar")]
        public string Gambar { get; set; }

        [Column("harga")]
        public int Harga { get; set; }

        [Column("stok")]
        public int Stok { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }

    public class KurangiStokDto
    {
        public int Jumlah { get; set; }
    }


}
