using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kafejka.Models
{
    public class Visit
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required, MaxLength(20)]
        public string VisitCode { get; set; }

        [Required, Column(TypeName = "decimal(10, 2)")]
        public decimal TotalPrice { get; set; }

        public DateTime VisitDate { get; set; }

        // Relacja: Jedna wizyta ma wiele pozycji i pieczątek
        public ICollection<VisitItem> VisitItems { get; set; }
    }
}
