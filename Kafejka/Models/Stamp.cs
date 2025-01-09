using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Kafejka.Models
{
    public class Stamp
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [Required]
        public int VisitId { get; set; }

        [ForeignKey(nameof(VisitId))]
        public Visit Visit { get; set; }
    }
}
