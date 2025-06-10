using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class RentalManagementModel : PageModel
    {
        private readonly IRentalService _rentalService;
        private readonly IVehicleService _vehicleService;
        private readonly IUserService _userService;
        private readonly ILogger<RentalManagementModel> _logger;

        public RentalManagementModel(
            IRentalService rentalService,
            IVehicleService vehicleService,
            IUserService userService,
            ILogger<RentalManagementModel> logger)
        {
            _rentalService = rentalService;
            _vehicleService = vehicleService;
            _userService = userService;
            _logger = logger;
        }

        [BindProperty(SupportsGet = true)]
        public string StatusFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? StartDateFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime? EndDateFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public int CurrentPage { get; set; } = 1;
        public int TotalPages { get; set; }
        public const int PageSize = 10;

        public List<RentalViewModel> Rentals { get; set; } = new();
        public List<ApplicationUser> Customers { get; set; } = new();
        public List<Vehicle> AvailableVehicles { get; set; } = new();

        [BindProperty]
        public CreateRentalDto NewRental { get; set; } = new();

        [BindProperty]
        public string ReportType { get; set; }
        [BindProperty]
        public DateTime ReportStartDate { get; set; } = DateTime.Now.AddMonths(-1);
        [BindProperty]
        public DateTime ReportEndDate { get; set; } = DateTime.Now;

        public async Task OnGetAsync()
        {
            try
            {
                var allUsers = await _userService.GetAllUsersAsync();
                Customers = allUsers.ToList();

                try
                {
                    AvailableVehicles = (await _vehicleService.GetAvailableVehiclesAsync()).ToList();
                }
                catch
                {
                    var allVehicles = await _vehicleService.GetAllVehiclesAsync();
                    AvailableVehicles = allVehicles.Where(v => v.Status == VehicleStatus.AVAILABLE).ToList();
                }

                var rentals = await _rentalService.GetFilteredRentalsAsync(
                    StatusFilter,
                    StartDateFilter,
                    EndDateFilter,
                    CurrentPage,
                    PageSize);

                var totalItems = await _rentalService.GetRentalsCountAsync(
                    StatusFilter,
                    StartDateFilter,
                    EndDateFilter);
                TotalPages = (int)Math.Ceiling(totalItems / (double)PageSize);

                Rentals = rentals.Select(r => new RentalViewModel
                {
                    Id = r.Id,
                    CustomerName = r.Customer?.UserName ?? "N/A",
                    VehicleModel = r.RentedVehicle?.Model ?? "N/A",
                    StartDate = r.StartTime,
                    EndDate = r.EndTime,
                    TotalCost = r.TotalCost,
                    Status = r.Status.ToString()
                }).ToList();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao carregar aluguéis");
                TempData["ErrorMessage"] = "Erro ao carregar aluguéis. Por favor, tente novamente.";
            }
        }

        public async Task<IActionResult> OnPostCreateAsync()
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    await OnGetAsync();
                    return Page();
                }

                var rental = new Rental
                {
                    CustomerId = NewRental.CustomerId,
                    VehicleId = NewRental.VehicleId,
                    StartTime = NewRental.StartDate,
                    EndTime = NewRental.EndDate,
                };

                if (Enum.TryParse<RentalStatus>(NewRental.Status, out var status))
                {
                    rental.Status = status;
                }

                await _rentalService.CreateRentalAsync(rental);
                TempData["SuccessMessage"] = "Aluguel criado com sucesso!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar aluguel");
                TempData["ErrorMessage"] = $"Erro ao criar aluguel: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCompleteAsync(int id)
        {
            try
            {
                await _rentalService.CompleteRentalAsync(id);
                TempData["SuccessMessage"] = "Aluguel finalizado com sucesso!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao finalizar aluguel");
                TempData["ErrorMessage"] = $"Erro ao finalizar aluguel: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostCancelAsync(int id)
        {
            try
            {
                await _rentalService.CancelRentalAsync(id);
                TempData["SuccessMessage"] = "Aluguel cancelado com sucesso!";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cancelar aluguel");
                TempData["ErrorMessage"] = $"Erro ao cancelar aluguel: {ex.Message}";
            }

            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostGenerateReportAsync()
        {
            try
            {
                var reportData = await _rentalService.GetFilteredRentalsAsync(
                    null,
                    ReportStartDate,
                    ReportEndDate,
                    1,
                    1000
                );

                TempData["SuccessMessage"] = "Relatório gerado com sucesso!";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao gerar relatório");
                TempData["ErrorMessage"] = $"Erro ao gerar relatório: {ex.Message}";
                return RedirectToPage();
            }
        }
    }

    public class RentalViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; }
        public string VehicleModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }
    }

    public class CreateRentalDto
    {
        public string CustomerId { get; set; }
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; } = "ACTIVE";
    }
}
