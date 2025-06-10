using Microsoft.AspNetCore.Mvc.RazorPages;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;
using System.Threading.Tasks;

namespace Wheels_in_Csharp.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly IVehicleService _vehicleService;

        public Vehicle Vehicle { get; set; }

        public DetailsModel(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        public async Task OnGetAsync(int id)
        {
            Vehicle = await _vehicleService.GetVehicleByIdAsync(id);
        }

        public string GetStatusBadgeClass(VehicleStatus status)
        {
            return status switch
            {
                VehicleStatus.AVAILABLE => "bg-success",
                VehicleStatus.RENTED => "bg-danger",
                VehicleStatus.MAINTENANCE => "bg-warning",
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
    }
}