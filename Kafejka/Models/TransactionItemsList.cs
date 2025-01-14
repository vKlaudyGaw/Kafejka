namespace Kafejka.Models
{
    public class TransactionItemsList
    {
        public int Id { get; set; }
        public int TransactionId { get; set; }
        public virtual Transaction? Transaction { get; set; }
        public int MenuItemId { get; set; }
        public virtual MenuItem? MenuItem {  get; set; }
    }
}
