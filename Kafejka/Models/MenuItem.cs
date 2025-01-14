using System.ComponentModel.DataAnnotations;

namespace Kafejka.Models
{
    public class MenuItem
    {
        public int Id { get; set; }

        [Display(Name = "Pozycja w menu")]
        public string Name { get; set; }
        public string Description { get; set; }

        //ceny w kawiarni są pełnymi złotówkami np.12zł, 4 zł dlatego int
        public int Price { get; set; }

        public  int ItemTypeId { get; set; }
        public virtual ItemType? Type { get; set; }
    }
}
