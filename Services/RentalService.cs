using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Wheels_in_Csharp.Data;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Services
{
    public class RentalService : IRentalService
    {
        private readonly ApplicationDbContext _context;
        private readonly IVehicleService _vehicleService;

        public RentalService(ApplicationDbContext context, IVehicleService vehicleService)
        {
            _context = context;
            _vehicleService = vehicleService;
        }

        public async Task<Rental> GetRentalByIdAsync(int id)
        {
            return await _context.Rentals
                .Include(r => r.Customer)
                .Include(r => r.RentedVehicle)
                .Include(r => r.PaymentInfo)
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task<IEnumerable<Rental>> GetAllRentalsAsync()
        {
            return await _context.Rentals
                .Include(r => r.Customer)
                .Include(r => r.RentedVehicle)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public IQueryable<Rental> GetAllRentalsQueryable()
        {
            return _context.Rentals
                .Include(r => r.Customer)
                .Include(r => r.RentedVehicle)
                .AsQueryable();
        }

        public async Task<IEnumerable<Rental>> GetUserRentalsAsync(string userId)
        {
            return await _context.Rentals
                .Include(r => r.RentedVehicle)
                .Where(r => r.CustomerId == userId)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetRentalsByVehicleAsync(int vehicleId)
        {
            return await _context.Rentals
                .Include(r => r.Customer)
                .Where(r => r.VehicleId == vehicleId)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<Rental> CreateRentalAsync(Rental rental)
        {
            if (rental.Id == 0) // New rental
            {
                if (!await IsVehicleAvailableForRentalAsync(rental.VehicleId, rental.StartTime, rental.EndTime))
                    throw new InvalidOperationException("Vehicle is not available for the selected period");

                rental.TotalCost = await CalculateRentalCostAsync(
                    rental.VehicleId, rental.StartTime, rental.EndTime);

                rental.Status = RentalStatus.ACTIVE;
                await _vehicleService.UpdateVehicleStatusAsync(rental.VehicleId, VehicleStatus.RENTED);

                _context.Rentals.Add(rental);
            }
            else // Existing rental
            {
                _context.Rentals.Update(rental);
            }

            await _context.SaveChangesAsync();
            return rental;
        }

        public async Task CompleteRentalAsync(int rentalId)
        {
            var rental = await GetRentalByIdAsync(rentalId);
            if (rental == null) throw new ArgumentException("Rental not found");

            if (rental.Status != RentalStatus.ACTIVE)
                throw new InvalidOperationException("Only active rentals can be completed");

            rental.Status = RentalStatus.COMPLETED;

            // MUDANÇA: Como não existe ActualEndTime, vamos usar EndTime como referência
            // Se quiser registrar o momento exato da finalização, atualize o EndTime
            rental.EndTime = DateTime.Now;

            // Recalculate cost based on actual usage
            var actualDuration = rental.EndTime - rental.StartTime;
            var hoursUsed = (decimal)Math.Ceiling(actualDuration.TotalHours);
            rental.TotalCost = hoursUsed * rental.RentedVehicle.HourlyRate;

            await _vehicleService.UpdateVehicleStatusAsync(rental.VehicleId, VehicleStatus.AVAILABLE);
            await _context.SaveChangesAsync();
        }

        public async Task CancelRentalAsync(int rentalId)
        {
            var rental = await GetRentalByIdAsync(rentalId);
            if (rental == null) throw new ArgumentException("Rental not found");

            if (rental.Status != RentalStatus.ACTIVE)
                throw new InvalidOperationException("Only active rentals can be cancelled");

            rental.Status = RentalStatus.CANCELLED;

            if (rental.StartTime > DateTime.UtcNow)
            {
                await _vehicleService.UpdateVehicleStatusAsync(rental.VehicleId, VehicleStatus.AVAILABLE);
            }

            await _context.SaveChangesAsync();
        }

        public async Task ExtendRentalAsync(int rentalId, DateTime newEndDate)
        {
            var rental = await GetRentalByIdAsync(rentalId);
            if (rental == null) throw new ArgumentException("Rental not found");
            if (newEndDate <= rental.EndTime) throw new ArgumentException("New end date must be after current end date");

            // Check vehicle availability for extension period
            if (!await IsVehicleAvailableForRentalAsync(rental.VehicleId, rental.EndTime, newEndDate, rentalId))
                throw new InvalidOperationException("Vehicle not available for the extended period");

            var additionalCost = await CalculateRentalCostAsync(
                rental.VehicleId, rental.EndTime, newEndDate);

            rental.TotalCost += additionalCost;
            rental.EndTime = newEndDate;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsVehicleAvailableForRentalAsync(int vehicleId, DateTime startDate, DateTime endDate, int? excludeRentalId = null)
        {
            if (!await _vehicleService.IsVehicleAvailableAsync(vehicleId))
                return false;

            var query = _context.Rentals
                .Where(r => r.VehicleId == vehicleId &&
                           r.Status == RentalStatus.ACTIVE);

            if (excludeRentalId.HasValue)
            {
                query = query.Where(r => r.Id != excludeRentalId.Value);
            }

            return !await query
                .AnyAsync(r => (startDate >= r.StartTime && startDate < r.EndTime) ||
                                (endDate > r.StartTime && endDate <= r.EndTime) ||
                                (startDate <= r.StartTime && endDate >= r.EndTime));
        }

        public async Task<decimal> CalculateRentalCostAsync(int vehicleId, DateTime startDate, DateTime endDate)
        {
            var vehicle = await _vehicleService.GetVehicleByIdAsync(vehicleId);
            if (vehicle == null) throw new ArgumentException("Vehicle not found");

            var duration = endDate - startDate;
            var hours = (decimal)Math.Ceiling(duration.TotalHours);
            return hours * vehicle.HourlyRate;
        }

        public async Task<IEnumerable<Rental>> GetFilteredRentalsAsync(
            string statusFilter = null,
            DateTime? startDateFilter = null,
            DateTime? endDateFilter = null,
            int pageNumber = 1,
            int pageSize = 10)
        {
            var query = GetAllRentalsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (Enum.TryParse<RentalStatus>(statusFilter, out var status))
                {
                    query = query.Where(r => r.Status == status);
                }
            }

            if (startDateFilter.HasValue)
            {
                query = query.Where(r => r.StartTime >= startDateFilter.Value);
            }

            if (endDateFilter.HasValue)
            {
                query = query.Where(r => r.EndTime <= endDateFilter.Value);
            }

            return await query
                .OrderByDescending(r => r.StartTime)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetRentalsCountAsync(
            string statusFilter = null,
            DateTime? startDateFilter = null,
            DateTime? endDateFilter = null)
        {
            var query = _context.Rentals.AsQueryable();

            if (!string.IsNullOrEmpty(statusFilter))
            {
                if (Enum.TryParse<RentalStatus>(statusFilter, out var status))
                {
                    query = query.Where(r => r.Status == status);
                }
            }

            if (startDateFilter.HasValue)
            {
                query = query.Where(r => r.StartTime >= startDateFilter.Value);
            }

            if (endDateFilter.HasValue)
            {
                query = query.Where(r => r.EndTime <= endDateFilter.Value);
            }

            return await query.CountAsync();
        }
    }
}