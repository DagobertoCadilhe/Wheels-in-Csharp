using Microsoft.AspNetCore.Mvc.RazorPages;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Memory;

namespace Wheels_in_CSharp.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IVehicleService _vehicleService;

        public IList<Vehicle> Vehicles { get; set; }

        public IndexModel(ILogger<IndexModel> logger, IVehicleService vehicleService)
        {
            _logger = logger;
            _vehicleService = vehicleService;
        }

        public void OnGet()
        {
            Vehicles = _vehicleService.ObterTodos();
        }
    }
}