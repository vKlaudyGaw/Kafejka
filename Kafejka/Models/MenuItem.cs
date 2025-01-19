using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kafejka.Models
{
    public class MenuItem
    {
        public int Id { get; set; }


        [Required(ErrorMessage = "Podaj nazwe produktu")]
        [Display(Name = "NAZWA")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Nazwa produktu musi mieć od 1 do 150 znaków")]
        public string Name { get; set; }


        [Required(ErrorMessage = "Podaj opis produktu")]
        [Display(Name = "OPIS")]
        [StringLength(1000, MinimumLength = 1, ErrorMessage = "Opis musi mieć od 1 do 1000 znaków")]
        public string Description { get; set; }


        [Required(ErrorMessage = "Podaj cenę produktu")]
        [Display(Name = "CENA")]
        [Range(0, 100000, ErrorMessage = "Cena nie może być ujemna lub zbyt duża.")]
        public int Price { get; set; }  //ceny pełnymi złotówkami np.12zł dlatego int

        [Display(Name = "RODZAJ")]
        [Required(ErrorMessage = "Wybierz typ produktu")]
        [ForeignKey("ItemType")]
        public  int ItemTypeId { get; set; }
        [Display(Name="RODZAJ")]
        public virtual ItemType? Type { get; set; }
    }
}
