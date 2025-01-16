using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kafejka.Models
{
    public class TransactionItemsList
    {
        public int Id { get; set; }

        [ForeignKey("Transaction")]
        [Display(Name = "Kod z paragonu")]
        public int TransactionId { get; set; }
        public virtual Transaction? Transaction { get; set; }

        [ForeignKey("MenuItem")]
        [Display(Name = "Pozycja w menu")]
        public int MenuItemId { get; set; }
        public virtual MenuItem? MenuItem {  get; set; }
    }
}
