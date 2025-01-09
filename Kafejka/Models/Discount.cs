using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kafejka.Models
{
    public class Discount
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required, MaxLength(20)]
        public string DiscountCode { get; set; }

        [Required, Column(TypeName = "decimal(5, 2)")]
        public decimal DiscountValue { get; set; }

        public DateTime ExpiryDate { get; set; }

        public bool Used { get; set; }
    }
}
