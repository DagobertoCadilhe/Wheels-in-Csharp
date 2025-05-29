using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wheels_in_Csharp.Models
{
    public abstract class Vehicle
    {
        public int Id { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo 'Model' obrigatório.")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Campo 'Model' deve conter entre 5 e 20 caracteres.")]
        public string Model { get; set; }

        [Required]
        public int Year { get; set; }

        public bool Available { get; set; } = true;

        [NotMapped]
        public string NomeSlug => Model?.ToLower().Replace(" ", "-");

        [DataType(DataType.Currency)]
        public decimal HourlyRate { get; set; }

        [Required]
        public string LicensePlate { get; set; }

        [Display(Name = "Descrição")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo 'Descrição' obrigatório.")]
        [StringLength(50, MinimumLength = 10, ErrorMessage = "Campo 'Descrição' deve conter entre 10 e 50 caracteres.")]
        public string Description { get; set; }

        [Display(Name = "Caminho da Imagem")]
        [Required(AllowEmptyStrings = false, ErrorMessage = "Campo 'Caminho da Image' obrigatório.")]
        public string ImagemUri { get; set; }

        [Display(Name = "Última Manutenção em")]
        [Required(ErrorMessage = "Campo 'Disponível em' obrigatório.")]
        [DataType("month")]
        [DisplayFormat(DataFormatString = "{0:MMMM \\de yyyy}")]
        public DateTime LastMaintenance { get; set; }

        public VehicleStatus Status { get; set; } = VehicleStatus.AVAILABLE;

        public abstract decimal CalculateRentalCost(DateTime startTime, DateTime endTime);
    }

    public enum VehicleStatus
    {
        AVAILABLE,
        RENTED,
        MAINTENANCE,
        DAMAGED,
        RETIRED
    }
}