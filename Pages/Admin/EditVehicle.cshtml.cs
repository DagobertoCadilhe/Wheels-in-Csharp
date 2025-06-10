using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;

namespace Wheels_in_Csharp.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class EditVehicleModel : PageModel
    {
        private readonly IVehicleService _vehicleService;
        private readonly ILogger<EditVehicleModel> _logger;

        public EditVehicleModel(
            IVehicleService vehicleService,
            ILogger<EditVehicleModel> logger)
        {
            _vehicleService = vehicleService;
            _logger = logger;
        }

        [BindProperty]
        public VehicleEditDto VehicleEdit { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(id);

            if (vehicle == null)
            {
                return NotFound();
            }

            VehicleEdit = new VehicleEditDto
            {
                Id = vehicle.Id,
                Model = vehicle.Model,
                Year = vehicle.Year,
                HourlyRate = vehicle.HourlyRate,
                LicensePlate = vehicle.LicensePlate,
                Description = vehicle.Description,
                ImagemUri = vehicle.ImagemUri,
                LastMaintenance = vehicle.LastMaintenance,
                Status = vehicle.Status,
                CurrentLocation = vehicle.CurrentLocation,
                Mileage = vehicle.Mileage,
                Color = vehicle.Color,
                Available = vehicle.Available
            };

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            try
            {
                var vehicle = await _vehicleService.GetVehicleByIdAsync(VehicleEdit.Id);
                if (vehicle == null)
                {
                    return NotFound();
                }

                // Atualiza a imagem apenas se uma nova URL foi fornecida
                if (!string.IsNullOrEmpty(VehicleEdit.ImagemUri))
                {
                    vehicle.ImagemUri = VehicleEdit.ImagemUri;
                }

                vehicle.Model = VehicleEdit.Model;
                vehicle.Year = VehicleEdit.Year;
                vehicle.HourlyRate = VehicleEdit.HourlyRate;
                vehicle.LicensePlate = VehicleEdit.LicensePlate;
                vehicle.Description = VehicleEdit.Description;
                vehicle.LastMaintenance = VehicleEdit.LastMaintenance;
                vehicle.Status = VehicleEdit.Status;
                vehicle.CurrentLocation = VehicleEdit.CurrentLocation;
                vehicle.Mileage = VehicleEdit.Mileage;
                vehicle.Color = VehicleEdit.Color;
                vehicle.Available = VehicleEdit.Available;

                await _vehicleService.UpdateVehicleAsync(vehicle);

                TempData["SuccessMessage"] = "Veículo atualizado com sucesso!";
                return RedirectToPage("./Vehicles");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Erro de concorrência ao atualizar veículo");
                ModelState.AddModelError("", "O veículo foi modificado por outro usuário. Por favor, recarregue a página e tente novamente.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao atualizar veículo");
                ModelState.AddModelError("", "Ocorreu um erro ao atualizar o veículo. Por favor, tente novamente.");
            }

            return Page();
        }
    }

    public class VehicleEditDto
    {
        public int Id { get; set; }
        public string Model { get; set; } = string.Empty;
        public int Year { get; set; }
        public decimal HourlyRate { get; set; }
        public string LicensePlate { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string ImagemUri { get; set; } = string.Empty;
        public DateTime LastMaintenance { get; set; }
        public VehicleStatus Status { get; set; }
        public string? CurrentLocation { get; set; }
        public int? Mileage { get; set; }
        public string? Color { get; set; }
        public bool Available { get; set; }
    }
}