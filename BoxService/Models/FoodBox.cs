using System.Collections.Generic;
using System.Data.Entity;

namespace BoxService.Models
{
    public class FoodBox
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Genre { get; set; }
        public decimal Price { get; set; }
        public ICollection<ApplicationUser> Users { get; set; }

    }
}