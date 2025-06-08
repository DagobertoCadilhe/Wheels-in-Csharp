using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Wheels_in_Csharp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required, PersonalData]
        public string FullName { get; set; }

        [Required, PersonalData, StringLength(11)]
        public string CPF { get; set; }

        [PersonalData]
        public string Address { get; set; }

        [PersonalData]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        public virtual ICollection<Rental> Rentals { get; set; }
        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; }
    }
}