using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wheels_in_Csharp.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }

        [Required]
        [StringLength(20)]
        public string Type { get; set; }

        [StringLength(100)]
        public string CardHolderName { get; set; }

        [StringLength(16, MinimumLength = 13)]
        [CreditCard]
        public string CardNumber { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime? ExpiryDate { get; set; }

        [StringLength(4, MinimumLength = 3)]
        public string SecurityCode { get; set; }

        [StringLength(100)]
        public string PixKey { get; set; }

        public bool IsPrimary { get; set; } = false;

        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public ICollection<Payment> Payments { get; set; }
    }

    public enum PaymentMethodType
    {
        CreditCard,
        DebitCard,
        PIX,
        Cash,
        BankSlip
    }
}
