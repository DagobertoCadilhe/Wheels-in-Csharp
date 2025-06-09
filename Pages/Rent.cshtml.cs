using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Pages
{
    [Authorize]
    public class RentModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly IRentalService _rentalService;
        private readonly ILogger<RentModel> _logger;

        public RentModel(
            IVehicleService vehicleService,
            IRentalService rentalService,
            ILogger<RentModel> logger)
        {
            _vehicleService = vehicleService;
            _rentalService = rentalService;
            _logger = logger;
        }

        public IEnumerable<Vehicle> AvailableVehicles { get; set; } = new List<Vehicle>();
        public string ErrorMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                AvailableVehicles = await _vehicleService.GetAvailableVehiclesAsync();
                _logger.LogInformation($"Carregados {AvailableVehicles.Count()} veículos disponíveis para aluguel");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar veículos disponíveis");
                ErrorMessage = "Erro ao carregar os veículos disponíveis. Tente novamente mais tarde.";
                AvailableVehicles = new List<Vehicle>();
            }
        }
    }
}