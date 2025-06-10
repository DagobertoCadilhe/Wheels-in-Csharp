using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Controllers
{
    [Authorize]
    [Route("api/rental")]
    [ApiController]
    public class RentalController : ControllerBase
    {
        private readonly IRentalService _rentalService;
        private readonly IVehicleService _vehicleService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RentalController> _logger;

        public RentalController(
            IRentalService rentalService,
            IVehicleService vehicleService,
            UserManager<ApplicationUser> userManager,
            ILogger<RentalController> logger)
        {
            _rentalService = rentalService;
            _vehicleService = vehicleService;
            _userManager = userManager;
            _logger = logger;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateRental([FromBody] CreateRentalRequest request)
        {
            try
            {
                _logger.LogInformation("Iniciando criação de aluguel...");

                if (request.StartDate >= request.EndDate)
                {
                    _logger.LogWarning("Data de início posterior ou igual à data de fim");
                    return BadRequest(new { message = "A data de início deve ser anterior à data de fim." });
                }

                if (request.StartDate < DateTime.UtcNow.AddMinutes(-5))
                {
                    _logger.LogWarning("Data de início no passado");
                    return BadRequest(new { message = "A data de início não pode ser no passado." });
                }

                var customerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (string.IsNullOrEmpty(customerId))
                {
                    _logger.LogWarning("Usuário não autenticado");
                    return Unauthorized(new { message = "Usuário não autenticado." });
                }

                var vehicle = await _vehicleService.GetVehicleByIdAsync(request.VehicleId);
                if (vehicle == null)
                {
                    _logger.LogWarning($"Veículo não encontrado: ID {request.VehicleId}");
                    return NotFound(new { message = "Veículo não encontrado." });
                }

                if (!await _rentalService.IsVehicleAvailableForRentalAsync(request.VehicleId, request.StartDate, request.EndDate))
                {
                    _logger.LogWarning($"Veículo {request.VehicleId} não disponível no período solicitado");
                    return BadRequest(new { message = "O veículo não está disponível para aluguel no período selecionado." });
                }

                var estimatedCost = await _rentalService.CalculateRentalCostAsync(request.VehicleId, request.StartDate, request.EndDate);
                if (estimatedCost <= 0)
                {
                    _logger.LogWarning($"Custo inválido calculado: {estimatedCost}");
                    return BadRequest(new { message = "Não foi possível calcular o custo do aluguel." });
                }

                var rental = new Rental
                {
                    CustomerId = customerId,
                    VehicleId = request.VehicleId,
                    StartTime = request.StartDate,
                    EndTime = request.EndDate,
                    TotalCost = estimatedCost,
                    Status = RentalStatus.ACTIVE
                };

                var createdRental = await _rentalService.CreateRentalAsync(rental);
                _logger.LogInformation($"Aluguel criado com sucesso: ID {createdRental.Id}");

                return Ok(new
                {
                    success = true,
                    rentalId = createdRental.Id,
                    message = "Aluguel criado com sucesso!"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar aluguel");
                return StatusCode(500, new { message = "Ocorreu um erro ao processar seu aluguel." });
            }
        }
    }

    public class CreateRentalRequest
    {
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
