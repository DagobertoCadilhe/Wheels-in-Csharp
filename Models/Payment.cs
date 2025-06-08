using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wheels_in_Csharp.Models
{
    public class Payment
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O valor do pagamento é obrigatório.")]
        [Range(0.01, 100000, ErrorMessage = "O valor deve estar entre R$ 0,01 e R$ 100.000,00")]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        [Required]
        [Display(Name = "Data do Pagamento")]
        [DataType(DataType.DateTime)]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Status")]
        public PaymentStatus Status { get; set; } = PaymentStatus.PENDING;

        [Display(Name = "Código da Transação")]
        [StringLength(50, ErrorMessage = "O código deve ter no máximo 50 caracteres.")]
        public string TransactionId { get; set; }

        [Display(Name = "Detalhes")]
        [StringLength(500, ErrorMessage = "Os detalhes devem ter no máximo 500 caracteres.")]
        public string Details { get; set; }

        // Relacionamento com Rental (1:1)
        [Display(Name = "Aluguel")]
        public int? RentalId { get; set; }

        [ForeignKey("RentalId")]
        public Rental Rental { get; set; }

        // Relacionamento com PaymentMethod
        [Display(Name = "Método de Pagamento")]
        public int? PaymentMethodId { get; set; }

        [ForeignKey("PaymentMethodId")]
        public PaymentMethod PaymentMethod { get; set; }

        // Relacionamento com ApplicationUser
        [Required]
        public string PayerId { get; set; }

        [ForeignKey("PayerId")]
        public ApplicationUser Payer { get; set; }
    }

    public enum PaymentStatus
    {
        [Display(Name = "Pendente")]
        PENDING,

        [Display(Name = "Concluído")]
        COMPLETED,

        [Display(Name = "Falhou")]
        FAILED,

        [Display(Name = "Reembolsado")]
        REFUNDED,

        [Display(Name = "Estornado")]
        REVERSED,

        [Display(Name = "Em Processamento")]
        PROCESSING
    }
}