using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Pages
{
    public class SearchModel : PageModel
    {
        private readonly ILogger<SearchModel> _logger;
        private readonly IVehicleService _vehicleService;

        public IList<Vehicle> Vehicles { get; set; } = new List<Vehicle>();

        public SearchModel(ILogger<SearchModel> logger, IVehicleService vehicleService)
        {
            _logger = logger;
            _vehicleService = vehicleService;
        }

        public async Task OnGetAsync(string searchTerm = null, string vehicleType = null)
        {
            try
            {
                if (string.IsNullOrEmpty(searchTerm) && string.IsNullOrEmpty(vehicleType))
                {
                    Vehicles = (await _vehicleService.GetAllVehiclesAsync()).ToList(); // Conversão explícita para IList
                }
                else
                {
                    Vehicles = (await _vehicleService.GetAvailableVehiclesAsync()).ToList(); // Conversão explícita para IList

                    // Aplicar filtros adicionais se necessário
                    if (!string.IsNullOrEmpty(searchTerm))
                    {
                        Vehicles = Vehicles.Where(v =>
                            v.Model.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                            v.Description.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                    }

                    if (!string.IsNullOrEmpty(vehicleType))
                    {
                        Vehicles = Vehicles.Where(v =>
                            (vehicleType == "Car" && v is Car) ||
                            (vehicleType == "Motorcycle" && v is Motorcycle) ||
                            (vehicleType == "Bicycle" && v is Bicycle))
                            .ToList();
                    }
                }

                _logger.LogInformation($"Retornados {Vehicles.Count} veículos na pesquisa");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao buscar veículos");
                TempData["ErrorMessage"] = "Ocorreu um erro ao buscar veículos. Por favor, tente novamente.";
            }
        }
    }
}