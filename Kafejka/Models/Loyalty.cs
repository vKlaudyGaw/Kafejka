using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kafejka.Models
{
    public class Loyalty
    {
        public int Id { get; set; }
        public int TotalPoints { get; set; } // Liczba punktów aktualna
        public int NumberOfStampsUses { get; set; } // Liczba wykorzystań 5 pieczątek
        public ICollection<Transaction> Transactions { get; set; }

        [NotMapped]
        public int CurrentPoints => TotalPoints % 100;//Niepotrzebna wartość

        [NotMapped]
        public int CurrentStamps => (TotalPoints / 100) - NumberOfStampsUses * 5;//Niepotrzebna wartość

        [NotMapped]
        public int FreeProductsAvailable => TotalPoints / 5; //Ilość dostępnych darmowych produktów

        [NotMapped]
        public MenuItem CurrentReward => CalculateCurrentReward();

        private MenuItem CalculateCurrentReward()
        {
            if (Transactions == null || !Transactions.Any())
                return null;

            var items = Transactions
                .SelectMany(t => t.TransactionItemsList)
                .GroupBy(i => i.MenuItemId)
                .OrderByDescending(g => g.Count())
                .FirstOrDefault();


            return items?.FirstOrDefault()?.MenuItem;
        }


        public string? UserId { get; set; }
        public IdentityUser? User { get; set; }

    }
}
