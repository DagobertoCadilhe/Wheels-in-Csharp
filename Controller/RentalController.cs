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

        public RentalController(
            IRentalService rentalService,
            IVehicleService vehicleService,
            UserManager<ApplicationUser> userManager)
        {
            _rentalService = rentalService;
            _vehicleService = vehicleService;
            _userManager = userManager;
        }

        [HttpPost("create")]
        public async Task<ActionResult<RentalResponse>> CreateRental([FromBody] CreateRentalRequest request)
        {
            try
            {
                // Validações básicas
                if (request.StartDate >= request.EndDate)
                {
                    return BadRequest("A data de início deve ser anterior à data de fim.");
                }

                if (request.StartDate < DateTime.UtcNow.AddMinutes(-5)) // Margem de 5 minutos
                {
                    return BadRequest("A data de início não pode ser no passado.");
                }

                // Obter o usuário atual
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return Unauthorized("Usuário não encontrado.");
                }

                // Verificar se o veículo existe e está disponível
                var vehicle = await _vehicleService.GetVehicleByIdAsync(request.VehicleId);
                if (vehicle == null)
                {
                    return NotFound("Veículo não encontrado.");
                }

                // Verificar disponibilidade para o período solicitado
                var isAvailable = await _rentalService.IsVehicleAvailableForRentalAsync(
                    request.VehicleId, request.StartDate, request.EndDate);

                if (!isAvailable)
                {
                    return BadRequest("O veículo não está disponível para o período solicitado.");
                }

                // Calcular o custo total
                var totalCost = await _rentalService.CalculateRentalCostAsync(
                    request.VehicleId, request.StartDate, request.EndDate);

                // Criar o aluguel
                var rental = new Rental
                {
                    CustomerId = userId,
                    VehicleId = request.VehicleId,
                    StartTime = request.StartDate,
                    EndTime = request.EndDate,
                    TotalCost = totalCost,
                    Status = RentalStatus.ACTIVE
                };

                var createdRental = await _rentalService.CreateRentalAsync(rental);

                var response = new RentalResponse
                {
                    Id = createdRental.Id,
                    VehicleModel = vehicle.Model,
                    StartDate = createdRental.StartTime,
                    EndDate = createdRental.EndTime,
                    TotalCost = createdRental.TotalCost,
                    Status = createdRental.Status.ToString(),
                    Message = "Aluguel criado com sucesso!"
                };

                return Ok(response);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro interno do servidor: " + ex.Message);
            }
        }

        [HttpPost("calculate-cost")]
        public async Task<ActionResult<decimal>> CalculateCost([FromBody] CalculateCostRequest request)
        {
            try
            {
                if (request.StartDate >= request.EndDate)
                {
                    return BadRequest("A data de início deve ser anterior à data de fim.");
                }

                var cost = await _rentalService.CalculateRentalCostAsync(
                    request.VehicleId, request.StartDate, request.EndDate);

                return Ok(new { TotalCost = cost });
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao calcular custo: " + ex.Message);
            }
        }

        [HttpGet("check-availability")]
        public async Task<ActionResult<bool>> CheckAvailability(
            int vehicleId, DateTime startDate, DateTime endDate)
        {
            try
            {
                var isAvailable = await _rentalService.IsVehicleAvailableForRentalAsync(
                    vehicleId, startDate, endDate);

                return Ok(new { IsAvailable = isAvailable });
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao verificar disponibilidade: " + ex.Message);
            }
        }

        [HttpGet("my-rentals")]
        public async Task<ActionResult<IEnumerable<RentalSummary>>> GetMyRentals()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var rentals = await _rentalService.GetUserRentalsAsync(userId);

                var rentalSummaries = rentals.Select(r => new RentalSummary
                {
                    Id = r.Id,
                    VehicleModel = r.RentedVehicle?.Model ?? "N/A",
                    StartDate = r.StartTime,
                    EndDate = r.EndTime,
                    TotalCost = r.TotalCost,
                    Status = r.Status.ToString()
                });

                return Ok(rentalSummaries);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Erro ao buscar aluguéis: " + ex.Message);
            }
        }

        [HttpPost("complete/{id}")]
        public async Task<ActionResult> CompleteRental(int id)
        {
            try
            {
                var rental = await _rentalService.GetRentalByIdAsync(id);
                if (rental == null)
                {
                    return NotFound("Aluguel não encontrado.");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (rental.CustomerId != userId)
                {
                    return Forbid("Você não tem permissão para completar este aluguel.");
                }

                await _rentalService.CompleteRentalAsync(id);
                return Ok(new { Message = "Aluguel finalizado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao finalizar aluguel: " + ex.Message);
            }
        }

        [HttpPost("cancel/{id}")]
        public async Task<ActionResult> CancelRental(int id)
        {
            try
            {
                var rental = await _rentalService.GetRentalByIdAsync(id);
                if (rental == null)
                {
                    return NotFound("Aluguel não encontrado.");
                }

                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (rental.CustomerId != userId)
                {
                    return Forbid("Você não tem permissão para cancelar este aluguel.");
                }

                await _rentalService.CancelRentalAsync(id);
                return Ok(new { Message = "Aluguel cancelado com sucesso!" });
            }
            catch (Exception ex)
            {
                return BadRequest("Erro ao cancelar aluguel: " + ex.Message);
            }
        }
    }

    // DTOs para as requisições e respostas
    public class CreateRentalRequest
    {
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class CalculateCostRequest
    {
        public int VehicleId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    public class RentalResponse
    {
        public int Id { get; set; }
        public string VehicleModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
    }

    public class RentalSummary
    {
        public int Id { get; set; }
        public string VehicleModel { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }
    }
}