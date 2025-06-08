using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Wheels_in_Csharp.Models
{
    public class Report
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "O tipo de relatório é obrigatório.")]
        [Display(Name = "Tipo de Relatório")]
        public ReportType Type { get; set; }

        [Required(ErrorMessage = "A data de início do período é obrigatória.")]
        [Display(Name = "Data Inicial")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "A data final do período é obrigatória.")]
        [Display(Name = "Data Final")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        [Display(Name = "Data de Geração")]
        [DataType(DataType.DateTime)]
        public DateTime GenerationDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Título do Relatório")]
        [StringLength(100, ErrorMessage = "O título deve ter no máximo 100 caracteres.")]
        public string Title { get; set; }

        [Display(Name = "Descrição")]
        [StringLength(500, ErrorMessage = "A descrição deve ter no máximo 500 caracteres.")]
        public string Description { get; set; }

        [Display(Name = "Caminho do Arquivo")]
        public string FilePath { get; set; }  // Para armazenar relatórios gerados em PDF/Excel

        [Display(Name = "Parâmetros Adicionais")]
        public string ParametersJson { get; set; }  // Armazena filtros/parâmetros em JSON

        // Relacionamento com o usuário que gerou o relatório
        [Required]
        public string GeneratedById { get; set; }

        [ForeignKey("GeneratedById")]
        public ApplicationUser GeneratedBy { get; set; }

        // Método para formatar o período do relatório
        [NotMapped]
        public string Period => $"{StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}";
    }

    public enum ReportType
    {
        [Display(Name = "Uso de Veículos")]
        VEHICLE_USAGE,

        [Display(Name = "Receitas Financeiras")]
        FINANCIAL_REVENUE,

        [Display(Name = "Disponibilidade da Frota")]
        FLEET_AVAILABILITY,

        [Display(Name = "Manutenções")]
        MAINTENANCE,

        [Display(Name = "Clientes Ativos")]
        ACTIVE_CUSTOMERS,

        [Display(Name = "Pagamentos")]
        PAYMENTS,

        [Display(Name = "Aluguéis")]
        RENTALS
    }

    // Classe auxiliar para parâmetros de relatório
    public class ReportParameters
    {
        public int? VehicleType { get; set; }
        public string StatusFilter { get; set; }
        public string LocationFilter { get; set; }
        public bool IncludeDetails { get; set; } = true;
    }
}