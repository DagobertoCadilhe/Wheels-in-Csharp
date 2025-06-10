using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class VehiclesModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<VehiclesModel> _logger;

        public VehiclesModel(IVehicleService vehicleService, ILogger<VehiclesModel> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string ModelFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string TypeFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public const int PageSize = 10;

        public List<Vehicle> Vehicles { get; set; } = new();

        public async Task OnGetAsync()
        {
            try
            {
                var vehiclesQuery = _vehicleService.GetAllVehiclesQueryable();

                if (!string.IsNullOrEmpty(ModelFilter))
                {
                    vehiclesQuery = vehiclesQuery.Where(v => v.Model.Contains(ModelFilter));
                }

                if (!string.IsNullOrEmpty(TypeFilter))
                {
                    vehiclesQuery = vehiclesQuery.Where(v => v.GetType().Name == TypeFilter);
                }

                if (!string.IsNullOrEmpty(StatusFilter) &&
                    Enum.TryParse<VehicleStatus>(StatusFilter, out var status))
                {
                    vehiclesQuery = vehiclesQuery.Where(v => v.Status == status);
                }

                var totalItems = await vehiclesQuery.CountAsync();
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

                Vehicles = await vehiclesQuery
                    .OrderBy(v => v.Model)
                    .Skip((CurrentPage - 1) * PageSize)
                    .Take(PageSize)
                    .ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar veículos");
                TempData["ErrorMessage"] = "Erro ao carregar veículos. Por favor, tente novamente.";
            }
        }

        public async Task<IActionResult> OnPostDeleteAsync(int id)
        {
            try
            {
                var vehicle = await _vehicleService.GetVehicleByIdAsync(id);
                if (vehicle == null)
                {
                    TempData["ErrorMessage"] = "Veículo não encontrado.";
                    return RedirectToPage();
                }

                await _vehicleService.DeleteVehicleAsync(id);
                TempData["SuccessMessage"] = $"Veículo '{vehicle.Model}' excluído com sucesso!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao excluir veículo");
                TempData["ErrorMessage"] = $"Erro ao excluir veículo: {ex.Message}";
            }

            return RedirectToPage(new
            {
                currentPage = CurrentPage,
                modelFilter = ModelFilter,
                typeFilter = TypeFilter,
                statusFilter = StatusFilter
            });
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
                VehicleStatus.MAINTENANCE => "Manutenção",
                VehicleStatus.DAMAGED => "Danificado",
                VehicleStatus.RETIRED => "Aposentado",
                _ => status.ToString()
            };
        }

        public string GetVehicleTypeName(Vehicle vehicle)
        {
            return vehicle switch
            {
                Car => "Carro",
                Motorcycle => "Moto",
                Bicycle => "Bicicleta",
                _ => "Veículo"
            };
        }
    }
}