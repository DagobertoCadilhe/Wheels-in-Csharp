using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wheels_in_Csharp.Models
{
    public abstract class Vehicle
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O modelo do veículo é obrigatório.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O modelo deve ter entre 2 e 100 caracteres.")]
        [Display(Name = "Modelo")]
        public string Model { get; set; }

        [Required(ErrorMessage = "O ano do veículo é obrigatório.")]
        [Range(1900, 2100, ErrorMessage = "O ano deve estar entre 1900 e 2100.")]
        [Display(Name = "Ano de Fabricação")]
        public int Year { get; set; }

        [Display(Name = "Disponível")]
        public bool Available { get; set; } = true;

        [NotMapped]
        [Display(Name = "Slug")]
        public string NomeSlug => Model?.ToLower().Replace(" ", "-");

        [Required(ErrorMessage = "O valor por hora é obrigatório.")]
        [Range(0.1, 1000, ErrorMessage = "O valor por hora deve estar entre R$ 0,10 e R$ 1.000,00")]
        [DataType(DataType.Currency)]
        [Display(Name = "Valor por Hora")]
        [Column(TypeName = "decimal(18,2)")]
        public decimal HourlyRate { get; set; }

        [Required(ErrorMessage = "A placa/license é obrigatória.")]
        [StringLength(15, ErrorMessage = "A placa deve ter no máximo 15 caracteres.")]
        [Display(Name = "Placa/Identificação")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "A descrição do veículo é obrigatória.")]
        [StringLength(500, MinimumLength = 20, ErrorMessage = "A descrição deve ter entre 20 e 500 caracteres.")]
        [Display(Name = "Descrição Detalhada")]
        public string Description { get; set; }

        [Required(ErrorMessage = "A imagem do veículo é obrigatória.")]
        [Display(Name = "URL da Imagem")]
        [Url(ErrorMessage = "Informe uma URL válida para a imagem.")]
        public string ImagemUri { get; set; }

        [Required(ErrorMessage = "A data da última manutenção é obrigatória.")]
        [Display(Name = "Última Manutenção")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:dd/MM/yyyy}")]
        public DateTime LastMaintenance { get; set; } = DateTime.UtcNow;

        [Required]
        [Display(Name = "Status do Veículo")]
        public VehicleStatus Status { get; set; } = VehicleStatus.AVAILABLE;

        [Display(Name = "Data de Cadastro")]
        [DataType(DataType.DateTime)]
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Localização Atual")]
        [StringLength(100, ErrorMessage = "A localização deve ter no máximo 100 caracteres.")]
        public string CurrentLocation { get; set; }

        [Display(Name = "Quilometragem")]
        [Range(0, 1000000, ErrorMessage = "A quilometragem deve estar entre 0 e 1.000.000")]
        public int? Mileage { get; set; }

        [Display(Name = "Cor")]
        [StringLength(30, ErrorMessage = "A cor deve ter no máximo 30 caracteres.")]
        public string Color { get; set; }

        public abstract decimal CalculateRentalCost(DateTime startTime, DateTime endTime);
    }

    public enum VehicleStatus
    {
        [Display(Name = "Disponível")]
        AVAILABLE,

        [Display(Name = "Alugado")]
        RENTED,

        [Display(Name = "Em Manutenção")]
        MAINTENANCE,

        [Display(Name = "Danificado")]
        DAMAGED,

        [Display(Name = "Aposentado")]
        RETIRED
    }
}