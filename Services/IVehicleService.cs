using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wheels_in_Csharp.Models;

namespace Wheels_in_Csharp.Services.Interfaces
{
    public interface IVehicleService
    {
        Task<IEnumerable<Vehicle>> GetAllVehiclesAsync();
        Task<Vehicle> GetVehicleByIdAsync(int id);
        Task<IEnumerable<Vehicle>> GetAvailableVehiclesAsync();
        Task<IEnumerable<Vehicle>> GetVehiclesByTypeAsync(string vehicleType);
        Task<Vehicle> AddVehicleAsync(Vehicle vehicle);
        Task UpdateVehicleAsync(Vehicle vehicle);
        Task DeleteVehicleAsync(int id);
        IQueryable<Vehicle> GetAllVehiclesQueryable();
        Task UpdateVehicleStatusAsync(int vehicleId, VehicleStatus status);
        Task<bool> IsVehicleAvailableAsync(int vehicleId);
        Task<decimal> CalculateRentalCostAsync(int vehicleId, DateTime startDate, DateTime endDate);
    }
}