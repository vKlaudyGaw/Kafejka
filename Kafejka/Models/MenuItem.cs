using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kafejka.Models
{
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }

        [Required]
        public string Category { get; set; } // Można użyć Enum

        [Required, Column(TypeName = "decimal(10, 2)")]
        public decimal Price { get; set; }

        public string Description { get; set; }

        
    }
}
