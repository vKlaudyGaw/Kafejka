using Microsoft.AspNetCore.Identity;

namespace Kafejka.Models
{
    public class Loyality
    {
        public int Id { get; set; }
        public int Points { get; set; }
        public int ConvertedPointsIntoStamps { get; set; }
        public int Stamps { get; set; }
        public int RedeemedStamps { get; set; }


        /*
        public int RewardId { get; set; }
        public virtual Reward? Reward { get; set; }
        */
    }
}
