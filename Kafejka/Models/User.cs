using System.ComponentModel.DataAnnotations;

namespace Kafejka.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Username { get; set; }

        [Required, MaxLength(255)]
        public string Password { get; set; }

        [Required, MaxLength(100)]
        public string Email { get; set; }

        public DateTime CreatedAt { get; set; }

        // Relacja: Jeden użytkownik ma wiele wizyt i pieczątek
        public ICollection<Visit> Visits { get; set; }
        public ICollection<Stamp> Stamps { get; set; }
        public ICollection<Discount> Discounts { get; set; }
    }
}
