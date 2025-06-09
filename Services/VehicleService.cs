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
    public class VehicleService : IVehicleService
    {
        private readonly ApplicationDbContext _context;

        public VehicleService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Vehicle>> GetAllVehiclesAsync()
        {
            return await _context.Vehicles.ToListAsync();
        }

        public async Task<Vehicle> GetVehicleByIdAsync(int id)
        {
            return await _context.Vehicles.FindAsync(id);
        }

        public async Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync()
        {
            return await _context.Vehicles
                .Where(v => v.Available && v.Status == VehicleStatus.AVAILABLE)
                .ToListAsync();
        }

        public async Task<IEnumerable<Vehicle>> GetVehiclesByTypeAsync(string vehicleType)
        {
            return await _context.Vehicles
                .Where(v => EF.Property<string>(v, "VehicleType") == vehicleType)
                .ToListAsync();
        }

        public async Task<Vehicle> AddVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Add(vehicle);
            await _context.SaveChangesAsync();
            return vehicle;
        }

        public async Task UpdateVehicleAsync(Vehicle vehicle)
        {
            _context.Vehicles.Update(vehicle);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteVehicleAsync(int id)
        {
            var vehicle = await _context.Vehicles.FindAsync(id);
            if (vehicle != null)
            {
                _context.Vehicles.Remove(vehicle);
                await _context.SaveChangesAsync();
            }
        }

        public async Task UpdateVehicleStatusAsync(int vehicleId, VehicleStatus status)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle != null)
            {
                vehicle.Status = status;
                vehicle.Available = status == VehicleStatus.AVAILABLE;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> IsVehicleAvailableAsync(int vehicleId)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            return vehicle?.Available == true && vehicle.Status == VehicleStatus.AVAILABLE;
        }

        public async Task<decimal> CalculateRentalCostAsync(int vehicleId, DateTime startDate, DateTime endDate)
        {
            var vehicle = await _context.Vehicles.FindAsync(vehicleId);
            if (vehicle == null) throw new ArgumentException("Vehicle not found");

            return vehicle.CalculateRentalCost(startDate, endDate);
        }

        public IQueryable<Vehicle> GetAllVehiclesQueryable()
        {
            return _context.Vehicles.AsQueryable();
        }
    }
}