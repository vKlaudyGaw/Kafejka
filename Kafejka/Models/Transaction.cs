using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Net.Mail;


namespace Kafejka.Models
{
    [Index(nameof(Code), IsUnique = true)] //zapobieganie dublikatom kodów, nikt nie użyje 2 razy tego samego kodu
    public class Transaction
    {
        public int Id { get; set; }


        [Display(Name = "Koszt")]
        [Required(ErrorMessage = "Podaj cenę z paragonu.")]
        [Range(1, 999999, ErrorMessage = "Cena nie może być ujemna lub jest zbyt duża.")]
        [RegularExpression("^\\d{1,6}$", ErrorMessage = "Podaj cenę w pełnych złotówkach z przedziału od 1 zł do 999999 zł")]
        public int Amount { get; set; }

        
        [Required(ErrorMessage = "Podaj kod z paragonu.")]
        [Display(Name = "Kod z paragonu")]
        [StringLength(10, MinimumLength = 10, ErrorMessage = "Kod musi mieć 10 znaków.")]
        [RegularExpression("^[a-zA-Z0-9]{10}$", ErrorMessage = "Kod musi zawierać tylko liczby lub litery oraz mieć 10 znaków.")]
        public string Code { get; set; }


        [Required(ErrorMessage = "Podaj datę zakupu z paragonu.")]
        [Display(Name = "Data zakupu")]
        public DateTime PurchaseTime { get; set; }


        //Required(ErrorMessage = "Zaznacz zamówione pozycje.")]
        [Display(Name = "Lista zakupionych pozycji")]
        public ICollection<TransactionItemsList> TransactionItemsList { get; set; }

    }
}
