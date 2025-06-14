using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace kopinang_api.Models
{
    [Table("ulasan")]
    public class Ulasan
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required]
        [Column("order_id")]
        public int OrderId { get; set; }

        [JsonIgnore]
        [ForeignKey("OrderId")]
        public Order? Order { get; set; }

        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = null!;  // Firebase UID

        [Required]
        [Range(1, 5)]
        [Column("rating")]
        public short Rating { get; set; }

        [Column("admin_reply")]
        public string? AdminReply { get; set; }

        [Column("review")]
        public string? Review { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }
    }

    public class AdminReplyDto
    {
        public string? AdminReply { get; set; }
    }

}
