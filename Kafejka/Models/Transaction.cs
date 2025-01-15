using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Kafejka.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int Amount { get; set; }
        
        [StringLength(20)]
        public string Code { get; set; }
        public DateTime PurchaseTime { get; set; }

        public ICollection<TransactionItemsList> TransactionItemsList { get; set; }
    
    }
}
