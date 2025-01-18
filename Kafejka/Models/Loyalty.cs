using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kafejka.Models
{
    public class Loyalty
    {
        public int Id { get; set; }
        public int TotalPoints { get; set; } // Całkowita liczba punktów
        public int NumberOfStampsUses { get; set; } // Liczba wykorzystań 5 pieczątek
        public ICollection<Transaction> Transactions { get; set; }

        [NotMapped]
        public int CurrentPoints => TotalPoints % 100;

        [NotMapped]
        public int CurrentStamps => (TotalPoints / 100) - NumberOfStampsUses * 5;

        [NotMapped]
        public int FreeProductsAvailable => CurrentStamps / 5;

        [NotMapped]
        public MenuItem CurrentReward => CalculateCurrentReward();

        private MenuItem CalculateCurrentReward()
        {
            // Jeśli nie ma transakcji, brak nagrody
            if (Transactions == null || !Transactions.Any())
                return null;

            // Pobranie listy zakupionych pozycji z transakcji
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
