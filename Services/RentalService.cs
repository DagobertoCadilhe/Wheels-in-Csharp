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
            if (!await _vehicleService.IsVehicleAvailableAsync(rental.VehicleId))
                throw new InvalidOperationException("Vehicle is not available for rental");

            rental.TotalCost = await _vehicleService.CalculateRentalCostAsync(
                rental.VehicleId, rental.StartTime, rental.EndTime);

            rental.Status = RentalStatus.ACTIVE;

            await _vehicleService.UpdateVehicleStatusAsync(rental.VehicleId, VehicleStatus.RENTED);

            _context.Rentals.Add(rental);
            await _context.SaveChangesAsync();

            return rental;
        }

        public async Task CompleteRentalAsync(int rentalId)
        {
            var rental = await GetRentalByIdAsync(rentalId);
            if (rental == null) throw new ArgumentException("Rental not found");

            rental.Status = RentalStatus.COMPLETED;
            await _vehicleService.UpdateVehicleStatusAsync(rental.VehicleId, VehicleStatus.AVAILABLE);

            await _context.SaveChangesAsync();
        }

        public async Task CancelRentalAsync(int rentalId)
        {
            var rental = await GetRentalByIdAsync(rentalId);
            if (rental == null) throw new ArgumentException("Rental not found");

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

            var additionalCost = await _vehicleService.CalculateRentalCostAsync(
                rental.VehicleId, rental.EndTime, newEndDate);

            rental.TotalCost += additionalCost;
            rental.EndTime = newEndDate;

            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsVehicleAvailableForRentalAsync(int vehicleId, DateTime startDate, DateTime endDate)
        {
            if (!await _vehicleService.IsVehicleAvailableAsync(vehicleId))
                return false;

            return !await _context.Rentals
                .AnyAsync(r => r.VehicleId == vehicleId &&
                               r.Status == RentalStatus.ACTIVE &&
                               ((startDate >= r.StartTime && startDate < r.EndTime) ||
                                (endDate > r.StartTime && endDate <= r.EndTime) ||
                                (startDate <= r.StartTime && endDate >= r.EndTime)));
        }

        public async Task<decimal> CalculateRentalCostAsync(int vehicleId, DateTime startDate, DateTime endDate)
        {
            return await _vehicleService.CalculateRentalCostAsync(vehicleId, startDate, endDate);
        }
    }
}