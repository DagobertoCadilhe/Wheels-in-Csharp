using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wheels_in_Csharp.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required]
        [Range(0.01, 100000)]
        [DataType(DataType.Currency)]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        public PaymentStatus Status { get; set; } = PaymentStatus.PENDING;

        [StringLength(50)]
        public string TransactionId { get; set; }

        [StringLength(500)]
        public string Details { get; set; }

        public int? RentalId { get; set; }

        [ForeignKey("RentalId")]
        public Rental Rental { get; set; }

        public int? PaymentMethodId { get; set; }

        [ForeignKey("PaymentMethodId")]
        public PaymentMethod PaymentMethod { get; set; }

        [Required]
        public string PayerId { get; set; }

        [ForeignKey("PayerId")]
        public ApplicationUser Payer { get; set; }
    }

    public enum PaymentStatus
    {
        PENDING,
        COMPLETED,
        FAILED,
        REFUNDED,
        REVERSED,
        PROCESSING
    }
}
