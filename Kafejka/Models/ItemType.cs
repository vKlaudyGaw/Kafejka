using System.ComponentModel.DataAnnotations;

namespace Kafejka.Models
{
    public class ItemType
    {
        [Display(Name="Rodzaj")]
        public int Id { get; set; }


        [Required(ErrorMessage = "Podaj nazwe rodzaju produktów")]
        [Display(Name = "Rodzaj")]
        [StringLength(150, MinimumLength = 1, ErrorMessage = "Nazwa rodzaju produktów musi mieć od 1 do 150 znaków")]
        public string Name { get; set; }
    }
}
