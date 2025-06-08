using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wheels_in_Csharp.Models
{
    public class PaymentMethod
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O tipo de pagamento é obrigatório.")]
        [Display(Name = "Tipo de Pagamento")]
        [StringLength(20, ErrorMessage = "O tipo deve ter no máximo 20 caracteres.")]
        public string Type { get; set; } // "CreditCard", "DebitCard", "PIX", "Cash", etc.

        [Display(Name = "Nome no Cartão")]
        [StringLength(100, ErrorMessage = "O nome no cartão deve ter no máximo 100 caracteres.")]
        public string CardHolderName { get; set; }

        [Display(Name = "Número do Cartão")]
        [StringLength(16, MinimumLength = 13, ErrorMessage = "O número do cartão deve ter entre 13 e 16 dígitos.")]
        [CreditCard(ErrorMessage = "Número de cartão inválido.")]
        public string CardNumber { get; set; }

        [Display(Name = "Data de Expiração")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:MM/yyyy}")]
        public DateTime? ExpiryDate { get; set; }

        [Display(Name = "Código de Segurança")]
        [StringLength(4, MinimumLength = 3, ErrorMessage = "O CVV deve ter 3 ou 4 dígitos.")]
        public string SecurityCode { get; set; }

        [Display(Name = "Chave PIX")]
        [StringLength(100, ErrorMessage = "A chave PIX deve ter no máximo 100 caracteres.")]
        public string PixKey { get; set; }

        [Display(Name = "Método Principal")]
        public bool IsPrimary { get; set; } = false;

        [Display(Name = "Data de Criação")]
        [DataType(DataType.DateTime)]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Relacionamento com ApplicationUser
        [Required]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User { get; set; }

        public ICollection<Payment> Payments { get; set; }
    }

    public enum PaymentMethodType
    {
        [Display(Name = "Cartão de Crédito")]
        CreditCard,

        [Display(Name = "Cartão de Débito")]
        DebitCard,

        [Display(Name = "PIX")]
        PIX,

        [Display(Name = "Dinheiro")]
        Cash,

        [Display(Name = "Boleto")]
        BankSlip
    }
}