using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Pages
{
    [Authorize]
    public class MyRentalsModel : PageModel
    {
        private readonly IRentalService _rentalService;
        private readonly ILogger<MyRentalsModel> _logger;

        public MyRentalsModel(IRentalService rentalService, ILogger<MyRentalsModel> logger)
        {
            _rentalService = rentalService;
            _logger = logger;
        }

        public IEnumerable<RentalSummaryViewModel> UserRentals { get; set; } = new List<RentalSummaryViewModel>();
        public string ErrorMessage { get; set; } = string.Empty;
        public string SuccessMessage { get; set; } = string.Empty;

        public async Task OnGetAsync()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    ErrorMessage = "Usuário não identificado.";
                    return;
                }

                await LoadUserRentals(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar aluguéis do usuário");
                ErrorMessage = "Ocorreu um erro ao carregar seus aluguéis. Por favor, tente novamente mais tarde.";
            }
        }

        public async Task<IActionResult> OnPostCompleteRentalAsync(int rentalId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                await _rentalService.CompleteRentalAsync(rentalId);

                SuccessMessage = "Veículo devolvido com sucesso!";
                await LoadUserRentals(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao completar aluguel");
                ErrorMessage = ex.Message;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostCancelRentalAsync(int rentalId)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized();
                }

                await _rentalService.CancelRentalAsync(rentalId);

                SuccessMessage = "Aluguel cancelado com sucesso!";
                await LoadUserRentals(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar aluguel");
                ErrorMessage = ex.Message;
            }

            return Page();
        }

        private async Task LoadUserRentals(string userId)
        {
            var rentals = await _rentalService.GetUserRentalsAsync(userId);

            UserRentals = rentals.Select(r => new RentalSummaryViewModel
            {
                Id = r.Id,
                VehicleModel = r.RentedVehicle?.Model ?? "Veículo não disponível",
                StartDate = r.StartTime,
                EndDate = r.EndTime,
                TotalCost = r.TotalCost,
                Status = r.Status.ToString(),
                CanBeReturned = r.Status == RentalStatus.ACTIVE && r.EndTime >= DateTime.Now,
                CanBeCanceled = r.Status == RentalStatus.ACTIVE && r.StartTime > DateTime.Now
            }).OrderByDescending(r => r.StartDate);
        }
    }

    public class RentalSummaryViewModel
    {
        public int Id { get; set; }
        public string VehicleModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }
        public bool CanBeReturned { get; set; }
        public bool CanBeCanceled { get; set; }
    }
}