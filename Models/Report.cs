using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wheels_in_Csharp.Models
{
    public class Report
    {
        public int Id { get; set; }

        [Required]
        public ReportType Type { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime GenerationDate { get; set; } = DateTime.UtcNow;

        [StringLength(100)]
        public string Title { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public string FilePath { get; set; }

        public string ParametersJson { get; set; }

        [Required]
        public string GeneratedById { get; set; }

        [ForeignKey("GeneratedById")]
        public ApplicationUser GeneratedBy { get; set; }

        [NotMapped]
        public string Period => $"{StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}";
    }

    public enum ReportType
    {
        VEHICLE_USAGE,
        FINANCIAL_REVENUE,
        FLEET_AVAILABILITY,
        MAINTENANCE,
        ACTIVE_CUSTOMERS,
        PAYMENTS,
        RENTALS
    }

    public class ReportParameters
    {
        public int? VehicleType { get; set; }
        public string StatusFilter { get; set; }
        public string LocationFilter { get; set; }
        public bool IncludeDetails { get; set; } = true;
    }
}
