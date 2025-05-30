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
            if (!ModelState.IsValid)
            {
                StatusList = new SelectList(_vehicleService.ObterTodosStatus());
                return Page();
            }

            Vehicle newVehicle = VehicleType switch
            {
                "Car" => Car,
                "Motorcycle" => Motorcycle,
                "Bicycle" => Bicycle,
                _ => throw new InvalidOperationException("Tipo de veículo desconhecido")
            };

            // Set common properties
            newVehicle.Status = Enum.Parse<VehicleStatus>(Request.Form["Status"]);

            _vehicleService.Incluir(newVehicle);

            return RedirectToPage("/Index");
        }
    }
}