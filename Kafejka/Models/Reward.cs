using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kafejka.Models
{
    //kody rabatowe, jaka zniżka będzie przyznana
    //100zł=1pieczątka 5pieczątek=1darmowa rzecz
    public class Reward
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required, MaxLength(20)]
        public string Code { get; set; }
        [Required]
        public int MenuItemId { get; set; }
        
        [ForeignKey(nameof(MenuItemId))]
        public MenuItem Item { get; set; }
        public bool Used { get; set; } //nie użyte=false, użyte=true
    }
}
