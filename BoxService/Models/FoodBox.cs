using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;

namespace BoxService.Models
{
    public class FoodBox
    {
        public int ID { get; set; }
        [Required]
        [StringLength(20)]
        public string Name { get; set; }
        public string Genre { get; set; }
        public int FlavorProfile { get; set; }
        [Required]
        public decimal Price { get; set; }
        public virtual ICollection<ApplicationUser> Users { get; set; }

    }
}