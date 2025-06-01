namespace kopinang_api.Models
{
    public class OrderDetail
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public Order Order { get; set; }

        public int ProdukId { get; set; }
        public Produk Produk { get; set; }

        public int Jumlah { get; set; }
        public int HargaSatuan { get; set; }
    }
}
