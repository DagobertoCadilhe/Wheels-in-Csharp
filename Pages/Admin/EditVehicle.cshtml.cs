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
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<EditVehicleModel> _logger;

        public EditVehicleModel(
            IVehicleService vehicleService,
            IWebHostEnvironment environment,
            ILogger<EditVehicleModel> logger)
        {
            _vehicleService = vehicleService;
            _environment = environment;
            _logger = logger;
        }

        [BindProperty]
        public VehicleEditDto VehicleEdit { get; set; }

        [BindProperty]
        public IFormFile ImageFile { get; set; }

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

                if (ImageFile != null && ImageFile.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads", "vehicles");
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var uniqueFileName = Guid.NewGuid().ToString() + "_" + ImageFile.FileName;
                    var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    vehicle.ImagemUri = "/uploads/vehicles/" + uniqueFileName;
                }
                else if (!string.IsNullOrEmpty(VehicleEdit.NewImageUrl))
                {
                    vehicle.ImagemUri = VehicleEdit.NewImageUrl;
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
}
