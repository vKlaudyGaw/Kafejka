using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Kafejka.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Display(Name = " ")]
        public string Name { get; set; }

        [Display(Name = "OPIS")]
        public string Description { get; set; }

        [Display(Name = "CENA")]
        public int Price { get; set; }  //ceny pełnymi złotówkami np.12zł dlatego int


        [ForeignKey("ItemType")]
        public  int ItemTypeId { get; set; }
        public virtual ItemType? Type { get; set; }
    }
}
