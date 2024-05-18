using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace RMS.Models
{
    public class AppUser : IdentityUser
    {

        [Required]
        [StringLength(255)]
        public string Name { get; set; } = null!;

        [Required]
        [StringLength(255)]
        public string Address { get; set; } = null!;

        public ICollection<Order> Orders { get; set; } = new List<Order>();
        public ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
        public ICollection<EmployeeBonus> Bonuses { get; set; } = new List<EmployeeBonus>();
    }
}
