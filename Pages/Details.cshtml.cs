using Wheels_in_Csharp.Models;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Wheels_in_CSharp.Services.Memory;

namespace Wheels_in_CSharp.Pages
{
    public class DetailsModel : PageModel
    {
        private IVehicleService _service;
        public Vehicle Vehicle { get; set; }

        public DetailsModel(IVehicleService vehicleService)
        {
            _service = vehicleService;
        }

        public void OnGet(int id)
        {
            Vehicle = _service.Obter(id);
        }
    }
}