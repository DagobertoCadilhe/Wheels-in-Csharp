using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Wheels_in_Csharp.Services.Memory;

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
        public Car Car { get; set; } = new Car();

        [BindProperty]
        public Motorcycle Motorcycle { get; set; } = new Motorcycle();

        [BindProperty]
        public Bicycle Bicycle { get; set; } = new Bicycle();

        [BindProperty]
        public string VehicleType { get; set; }

        [BindProperty]
        public string Model { get; set; }

        [BindProperty]
        public string Description { get; set; }

        [BindProperty]
        public int Year { get; set; }

        [BindProperty]
        public decimal HourlyRate { get; set; }

        [BindProperty]
        public string LicensePlate { get; set; }

        [BindProperty]
        public DateTime? LastMaintenance { get; set; }

        [BindProperty]
        public VehicleStatus Status { get; set; }

        [BindProperty]
        public string ImagemUri { get; set; }

        public SelectList VehicleTypes { get; set; } = new SelectList(new[]
        {
            new { Id = "Car", Name = "Carro" },
            new { Id = "Motorcycle", Name = "Moto" },
            new { Id = "Bicycle", Name = "Bicicleta" }
        }, "Id", "Name");

        public SelectList StatusList { get; set; }

        public void OnGet()
        {
            StatusList = new SelectList(_vehicleService.ObterTodosStatus());
        }

        public IActionResult OnPost()
        {
            // Debug: Verificar se os dados estão chegando
            Console.WriteLine($"VehicleType: {VehicleType}");
            Console.WriteLine($"Model: {Model}");
            Console.WriteLine($"Year: {Year}");
            Console.WriteLine($"HourlyRate: {HourlyRate}");

            // REMOVENDO COMPLETAMENTE A VALIDAÇÃO DO MODELSTATE
            // Vamos processar diretamente sem validar

            try
            {
                Vehicle newVehicle = VehicleType switch
                {
                    "Car" => CreateCar(),
                    "Motorcycle" => CreateMotorcycle(),
                    "Bicycle" => CreateBicycle(),
                    _ => CreateGenericVehicle() // Fallback para veículo genérico
                };

                // Debug: Verificar veículo criado
                Console.WriteLine($"Veículo criado: {newVehicle.Model}, Ano: {newVehicle.Year}");

                // Contar veículos antes da inclusão
                var totalAntes = _vehicleService.ObterTodos().Count;

                _vehicleService.Incluir(newVehicle);

                // Verificar se foi incluído contando novamente
                var totalDepois = _vehicleService.ObterTodos().Count;
                Console.WriteLine($"Total de veículos: {totalAntes} -> {totalDepois}");

                if (totalDepois > totalAntes)
                {
                    // Sucesso - veículo foi incluído
                    var tipoVeiculo = VehicleType switch
                    {
                        "Car" => "Carro",
                        "Motorcycle" => "Moto",
                        "Bicycle" => "Bicicleta",
                        _ => "Veículo"
                    };

                    TempData["SuccessMessage"] = $"{tipoVeiculo} '{Model ?? "Sem modelo"}' foi cadastrado com sucesso! Total de veículos: {totalDepois}";
                    TempData["VehicleModel"] = Model ?? "Sem modelo";
                    TempData["VehicleType"] = tipoVeiculo;
                }
                else
                {
                    // Erro - veículo não foi incluído
                    TempData["ErrorMessage"] = "Erro: O veículo não foi incluído na lista. Tente novamente.";
                    Console.WriteLine("ERRO: Veículo não foi incluído - contagem não aumentou");
                }

                return RedirectToPage("/Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao criar veículo: {ex.Message}");
                TempData["ErrorMessage"] = $"Erro ao cadastrar veículo: {ex.Message}";
                StatusList = new SelectList(_vehicleService.ObterTodosStatus());
                return Page();
            }
        }

        private Car CreateCar()
        {
            return new Car
            {
                Model = Model ?? "Modelo não informado",
                Description = Description ?? "",
                Year = Year > 0 ? Year : DateTime.Now.Year,
                HourlyRate = HourlyRate > 0 ? HourlyRate : 0,
                LicensePlate = LicensePlate ?? "",
                LastMaintenance = LastMaintenance ?? DateTime.Now,
                Status = Status,
                ImagemUri = ImagemUri ?? "",
                Available = Status == VehicleStatus.AVAILABLE,
                FuelType = Car.FuelType ?? "Não informado",
                Seats = Car.Seats > 0 ? Car.Seats : 4, // Default para 4 assentos
                Transmission = Car.Transmission ?? "Não informado",
                HasAC = Car.HasAC
            };
        }

        private Motorcycle CreateMotorcycle()
        {
            return new Motorcycle
            {
                Model = Model ?? "Modelo não informado",
                Description = Description ?? "",
                Year = Year > 0 ? Year : DateTime.Now.Year,
                HourlyRate = HourlyRate > 0 ? HourlyRate : 0,
                LicensePlate = LicensePlate ?? "",
                LastMaintenance = LastMaintenance ?? DateTime.Now,
                Status = Status,
                ImagemUri = ImagemUri ?? "",
                Available = Status == VehicleStatus.AVAILABLE,
                EngineCapacity = Motorcycle.EngineCapacity > 0 ? Motorcycle.EngineCapacity : 150, // Default 150cc
                HasHelmet = Motorcycle.HasHelmet
            };
        }

        private Bicycle CreateBicycle()
        {
            return new Bicycle
            {
                Model = Model ?? "Modelo não informado",
                Description = Description ?? "",
                Year = Year > 0 ? Year : DateTime.Now.Year,
                HourlyRate = HourlyRate > 0 ? HourlyRate : 0,
                LicensePlate = LicensePlate ?? "",
                LastMaintenance = LastMaintenance ?? DateTime.Now,
                Status = Status,
                ImagemUri = ImagemUri ?? "",
                Available = Status == VehicleStatus.AVAILABLE,
                BikeType = Bicycle.BikeType ?? "Bicicleta comum",
                HasHelmet = Bicycle.HasHelmet,
                HasLock = Bicycle.HasLock
            };
        }

        // Método para criar veículo genérico quando tipo não é especificado
        private Vehicle CreateGenericVehicle()
        {
            return new Car // Usando Car como base para veículo genérico
            {
                Model = Model ?? "Veículo genérico",
                Description = Description ?? "",
                Year = Year > 0 ? Year : DateTime.Now.Year,
                HourlyRate = HourlyRate > 0 ? HourlyRate : 0,
                LicensePlate = LicensePlate ?? "",
                LastMaintenance = LastMaintenance ?? DateTime.Now,
                Status = Status,
                ImagemUri = ImagemUri ?? "",
                Available = Status == VehicleStatus.AVAILABLE,
                FuelType = "Não informado",
                Seats = 4,
                Transmission = "Não informado",
                HasAC = false
            };
        }
    }
}