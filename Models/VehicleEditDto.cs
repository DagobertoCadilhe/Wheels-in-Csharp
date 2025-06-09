using System.ComponentModel.DataAnnotations;

namespace Wheels_in_Csharp.Models
{
    public class VehicleEditDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O modelo do veículo é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O modelo deve ter entre 2 e 100 caracteres.")]
        public string Model { get; set; }

        [Required(ErrorMessage = "O ano do veículo é obrigatório.")]
        [Range(1900, 2100, ErrorMessage = "O ano deve estar entre 1900 e 2100.")]
        public int Year { get; set; }

        [Required(ErrorMessage = "O valor por hora é obrigatório.")]
        [Range(0.1, 1000, ErrorMessage = "O valor por hora deve estar entre R$ 0,10 e R$ 1.000,00")]
        public decimal HourlyRate { get; set; }

        [Required(ErrorMessage = "A placa/license é obrigatória.")]
        [StringLength(15, ErrorMessage = "A placa deve ter no máximo 15 caracteres.")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "A descrição do veículo é obrigatória.")]
        [StringLength(500, MinimumLength = 20, ErrorMessage = "A descrição deve ter entre 20 e 500 caracteres.")]
        public string Description { get; set; }

        [Display(Name = "URL da Imagem")]
        [Url(ErrorMessage = "Informe uma URL válida para a imagem.")]
        public string ImagemUri { get; set; }

        [Required(ErrorMessage = "A data da última manutenção é obrigatória.")]
        public DateTime LastMaintenance { get; set; }

        [Required]
        public VehicleStatus Status { get; set; }

        [StringLength(100, ErrorMessage = "A localização deve ter no máximo 100 caracteres.")]
        public string CurrentLocation { get; set; }

        [Range(0, 1000000, ErrorMessage = "A quilometragem deve estar entre 0 e 1.000.000")]
        public int? Mileage { get; set; }

        [StringLength(30, ErrorMessage = "A cor deve ter no máximo 30 caracteres.")]
        public string Color { get; set; }

        [Display(Name = "Nova URL da Imagem")]
        [Url(ErrorMessage = "Informe uma URL válida para a imagem.")]
        public string NewImageUrl { get; set; }

        public bool Available { get; set; }
    }
}