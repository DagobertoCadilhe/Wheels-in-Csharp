using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Pages
{
    [Authorize]
    public class RentVehicleModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        public Vehicle Vehicle { get; set; }
        public string ErrorMessage { get; set; }

        public RentVehicleModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            try
            {
                Vehicle = await _vehicleService.GetVehicleByIdAsync(id);

                if (Vehicle == null)
                {
                    ErrorMessage = "Veículo não encontrado.";
                    return NotFound();
                }

                return Page();
            }
            catch (Exception ex)
            {
                ErrorMessage = "Erro ao carregar informações do veículo: " + ex.Message;
                return Page();
            }
        }

        // Métodos auxiliares para a view
        public string GetStatusBadgeClass(VehicleStatus status)
        {
            return status switch
            {
                VehicleStatus.AVAILABLE => "bg-success",
                VehicleStatus.RENTED => "bg-danger",
                VehicleStatus.MAINTENANCE => "bg-warning text-dark",
                VehicleStatus.DAMAGED => "bg-dark",
                VehicleStatus.RETIRED => "bg-secondary",
                _ => "bg-info"
            };
        }

        public string GetStatusDisplayName(VehicleStatus status)
        {
            return status switch
            {
                VehicleStatus.AVAILABLE => "Disponível",
                VehicleStatus.RENTED => "Alugado",
                VehicleStatus.MAINTENANCE => "Em Manutenção",
                VehicleStatus.DAMAGED => "Danificado",
                VehicleStatus.RETIRED => "Retirado",
                _ => status.ToString()
            };
        }

        public string GetVehicleTypeDisplayName()
        {
            return Vehicle switch
            {
                Car => "Carro",
                Motorcycle => "Motocicleta",
                Bicycle => "Bicicleta",
                _ => "Veículo"
            };
        }

        public string GetVehicleIcon()
        {
            return Vehicle switch
            {
                Car => "fas fa-car",
                Motorcycle => "fas fa-motorcycle",
                Bicycle => "fas fa-bicycle",
                _ => "fas fa-car"
            };
        }
    }
}