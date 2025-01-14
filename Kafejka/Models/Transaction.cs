using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Kafejka.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        public string Code { get; set; }
        public int TransactionItemsListId { get; set; }
        public virtual TransactionItemsList? TransactionItemList { get; set;}
    }
}
