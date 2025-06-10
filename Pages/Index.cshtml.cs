using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IVehicleService _vehicleService;

        public IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public IList<Vehicle> FeaturedVehicles { get; set; } = new List<Vehicle>();

        public IndexModel(ILogger<IndexModel> logger, IVehicleService vehicleService)
        {
            _logger = logger;
            _vehicleService = vehicleService;
        }

        public async Task OnGetAsync()
        {
            try
            {
                var allVehicles = await _vehicleService.GetAvailableVehiclesAsync();

                Vehicles = allVehicles
                    .OrderByDescending(v => v.Year)
                    .ThenBy(v => v.Model)
                    .ToList();

                FeaturedVehicles = allVehicles
                    .OrderByDescending(v => v is Car)
                    .ThenByDescending(v => v.Year)
                    .Take(3)
                    .ToList();

                _logger.LogInformation($"Página inicial carregada com {Vehicles.Count} veículos");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar veículos para página inicial");
                TempData["ErrorMessage"] = "Ocorreu um erro ao carregar os veículos. Por favor, tente novamente mais tarde.";
            }
        }
    }
}