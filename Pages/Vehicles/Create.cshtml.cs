using Wheels_in_Csharp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Wheels_in_Csharp.Services.Interfaces;
using Wheels_in_Csharp.Models;
using System.Threading.Tasks;
using System.Linq;
using System.ComponentModel.DataAnnotations;

namespace Wheels_in_Csharp.Pages
{
    public class CreateModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        public CreateModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        [BindProperty]
        [Required(ErrorMessage = "Selecione o tipo de veículo")]
        public string VehicleType { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "O modelo é obrigatório")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "O modelo deve ter entre 2 e 100 caracteres")]
        public string Model { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "A descrição é obrigatória")]
        [StringLength(500, MinimumLength = 20, ErrorMessage = "A descrição deve ter entre 20 e 500 caracteres")]
        public string Description { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "O ano é obrigatório")]
        [Range(1900, 2100, ErrorMessage = "O ano deve estar entre 1900 e 2100")]
        public int Year { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "O valor por hora é obrigatório")]
        [Range(0.1, 1000, ErrorMessage = "O valor por hora deve estar entre R$ 0,10 e R$ 1.000,00")]
        public decimal HourlyRate { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "A placa é obrigatória")]
        [StringLength(15, ErrorMessage = "A placa deve ter no máximo 15 caracteres")]
        public string LicensePlate { get; set; } = string.Empty;

        [BindProperty]
        [Required(ErrorMessage = "A data da última manutenção é obrigatória")]
        public DateTime LastMaintenance { get; set; } = DateTime.Now;

        [BindProperty]
        public VehicleStatus Status { get; set; } = VehicleStatus.AVAILABLE;

        [BindProperty]
        [Required(ErrorMessage = "A URL da imagem é obrigatória")]
        [Url(ErrorMessage = "Informe uma URL válida")]
        public string ImagemUri { get; set; } = string.Empty;

        [BindProperty]
        public string? CurrentLocation { get; set; }

        [BindProperty]
        [Range(0, 1000000, ErrorMessage = "A quilometragem deve estar entre 0 e 1.000.000")]
        public int? Mileage { get; set; }

        [BindProperty]
        [StringLength(30, ErrorMessage = "A cor deve ter no máximo 30 caracteres")]
        public string? Color { get; set; }

        // Propriedades específicas para Car
        [BindProperty]
        public string? FuelType { get; set; }

        [BindProperty]
        [Range(1, 8, ErrorMessage = "O número de assentos deve estar entre 1 e 8")]
        public int Seats { get; set; } = 4;

        [BindProperty]
        public string? Transmission { get; set; }

        [BindProperty]
        public bool HasAC { get; set; }

        // Propriedades específicas para Motorcycle
        [BindProperty]
        [Range(50, 2000, ErrorMessage = "A capacidade do motor deve estar entre 50cc e 2000cc")]
        public int EngineCapacity { get; set; } = 150;

        [BindProperty]
        public bool HasHelmet { get; set; }

        // Propriedades específicas para Bicycle
        [BindProperty]
        public string? BikeType { get; set; }

        [BindProperty]
        public bool HasLock { get; set; }

        public SelectList VehicleTypes { get; set; } = new SelectList(new[]
        {
            new { Id = "Car", Name = "Carro" },
            new { Id = "Motorcycle", Name = "Moto" },
            new { Id = "Bicycle", Name = "Bicicleta" }
        }, "Id", "Name");

        public SelectList StatusList { get; set; } = new SelectList(new List<object>());

        public async Task OnGetAsync()
        {
            InitializeSelectLists();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                InitializeSelectLists();
                return Page();
            }

            try
            {
                Vehicle newVehicle = CreateVehicleByType();

                if (newVehicle == null)
                {
                    ModelState.AddModelError("VehicleType", "Tipo de veículo inválido");
                    InitializeSelectLists();
                    return Page();
                }

                await _vehicleService.AddVehicleAsync(newVehicle);

                string vehicleTypeName = GetVehicleTypeName(VehicleType);
                TempData["SuccessMessage"] = $"{vehicleTypeName} '{Model}' cadastrado com sucesso!";
                TempData["VehicleModel"] = Model;
                TempData["VehicleType"] = vehicleTypeName;

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Erro ao cadastrar veículo: {ex.Message}");
                InitializeSelectLists();
                return Page();
            }
        }

        private void InitializeSelectLists()
        {
            StatusList = new SelectList(
                Enum.GetValues(typeof(VehicleStatus))
                    .Cast<VehicleStatus>()
                    .Select(v => new {
                        Value = v.ToString(),
                        Text = GetStatusDisplayName(v)
                    }),
                "Value",
                "Text"
            );
        }

        private string GetStatusDisplayName(VehicleStatus status)
        {
            return status switch
            {
                VehicleStatus.AVAILABLE => "Disponível",
                VehicleStatus.RENTED => "Alugado",
                VehicleStatus.MAINTENANCE => "Em Manutenção",
                VehicleStatus.DAMAGED => "Danificado",
                VehicleStatus.RETIRED => "Aposentado",
                _ => status.ToString()
            };
        }

        private string GetVehicleTypeName(string vehicleType)
        {
            return vehicleType switch
            {
                "Car" => "Carro",
                "Motorcycle" => "Moto",
                "Bicycle" => "Bicicleta",
                _ => "Veículo"
            };
        }

        private Vehicle? CreateVehicleByType()
        {
            var baseProperties = new
            {
                Model,
                Description,
                Year,
                HourlyRate,
                LicensePlate,
                LastMaintenance,
                Status,
                ImagemUri,
                CurrentLocation,
                Mileage,
                Color,
                Available = Status == VehicleStatus.AVAILABLE,
                RegistrationDate = DateTime.UtcNow
            };

            return VehicleType switch
            {
                "Car" => new Car
                {
                    Model = baseProperties.Model,
                    Description = baseProperties.Description,
                    Year = baseProperties.Year,
                    HourlyRate = baseProperties.HourlyRate,
                    LicensePlate = baseProperties.LicensePlate,
                    LastMaintenance = baseProperties.LastMaintenance,
                    Status = baseProperties.Status,
                    ImagemUri = baseProperties.ImagemUri,
                    CurrentLocation = baseProperties.CurrentLocation,
                    Mileage = baseProperties.Mileage,
                    Color = baseProperties.Color,
                    Available = baseProperties.Available,
                    RegistrationDate = baseProperties.RegistrationDate,
                    FuelType = FuelType ?? "Não informado",
                    Seats = Seats,
                    Transmission = Transmission ?? "Não informado",
                    HasAC = HasAC
                },
                "Motorcycle" => new Motorcycle
                {
                    Model = baseProperties.Model,
                    Description = baseProperties.Description,
                    Year = baseProperties.Year,
                    HourlyRate = baseProperties.HourlyRate,
                    LicensePlate = baseProperties.LicensePlate,
                    LastMaintenance = baseProperties.LastMaintenance,
                    Status = baseProperties.Status,
                    ImagemUri = baseProperties.ImagemUri,
                    CurrentLocation = baseProperties.CurrentLocation,
                    Mileage = baseProperties.Mileage,
                    Color = baseProperties.Color,
                    Available = baseProperties.Available,
                    RegistrationDate = baseProperties.RegistrationDate,
                    EngineCapacity = EngineCapacity,
                    HasHelmet = HasHelmet
                },
                "Bicycle" => new Bicycle
                {
                    Model = baseProperties.Model,
                    Description = baseProperties.Description,
                    Year = baseProperties.Year,
                    HourlyRate = baseProperties.HourlyRate,
                    LicensePlate = baseProperties.LicensePlate,
                    LastMaintenance = baseProperties.LastMaintenance,
                    Status = baseProperties.Status,
                    ImagemUri = baseProperties.ImagemUri,
                    CurrentLocation = baseProperties.CurrentLocation,
                    Mileage = baseProperties.Mileage,
                    Color = baseProperties.Color,
                    Available = baseProperties.Available,
                    RegistrationDate = baseProperties.RegistrationDate,
                    BikeType = BikeType ?? "Bicicleta comum",
                    HasHelmet = HasHelmet,
                    HasLock = HasLock
                },
                _ => null
            };
        }
    }
}