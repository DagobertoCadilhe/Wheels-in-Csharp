using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Wheels_in_Csharp.Models;

namespace Wheels_in_Csharp.Services.Interfaces
{
    public interface IRentalService
    {
        Task<Rental> GetRentalByIdAsync(int id);
        Task<IEnumerable<Rental>> GetAllRentalsAsync();
        Task<IEnumerable<Rental>> GetUserRentalsAsync(string userId);
        Task<IEnumerable<Rental>> GetRentalsByVehicleAsync(int vehicleId);
        Task<Rental> CreateRentalAsync(Rental rental);
        Task CompleteRentalAsync(int rentalId);
        Task CancelRentalAsync(int rentalId);
        Task ExtendRentalAsync(int rentalId, DateTime newEndDate);
        Task<bool> IsVehicleAvailableForRentalAsync(int vehicleId, DateTime startDate, DateTime endDate);
        Task<decimal> CalculateRentalCostAsync(int vehicleId, DateTime startDate, DateTime endDate);
    }
}