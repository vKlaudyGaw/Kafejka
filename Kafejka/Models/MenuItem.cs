using System.ComponentModel.DataAnnotations;

namespace Kafejka.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Display(Name = " ")]
        public string Name { get; set; }

        [Display(Name = "OPIS")]
        public string Description { get; set; }

        //ceny w kawiarni są pełnymi złotówkami np.12zł, 4 zł dlatego int

        [Display(Name = "CENA")]
        public int Price { get; set; }

        public  int ItemTypeId { get; set; }
        public virtual ItemType? Type { get; set; }
    }
}
