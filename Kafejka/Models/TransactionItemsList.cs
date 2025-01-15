using System.ComponentModel.DataAnnotations.Schema;

namespace Kafejka.Models
{
    public class TransactionItemsList
    {
        public int Id { get; set; }

        [ForeignKey("Transaction")]
        public int TransactionId { get; set; }
        public virtual Transaction? Transaction { get; set; }

        [ForeignKey("MenuItem")]
        public int MenuItemId { get; set; }
        public virtual MenuItem? MenuItem {  get; set; }
    }
}
