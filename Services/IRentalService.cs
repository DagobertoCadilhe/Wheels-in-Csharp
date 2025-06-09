using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Wheels_in_Csharp.Models;

namespace Wheels_in_Csharp.Services.Interfaces
{
    public interface IRentalService
    {
        // Métodos de consulta básicos
        Task<Rental> GetRentalByIdAsync(int id);
        Task<IEnumerable<Rental>> GetAllRentalsAsync();
        IQueryable<Rental> GetAllRentalsQueryable();
        Task<IEnumerable<Rental>> GetUserRentalsAsync(string userId);
        Task<IEnumerable<Rental>> GetRentalsByVehicleAsync(int vehicleId);

        // Operações de CRUD
        Task<Rental> CreateRentalAsync(Rental rental);
        Task CompleteRentalAsync(int rentalId);
        Task CancelRentalAsync(int rentalId);
        Task ExtendRentalAsync(int rentalId, DateTime newEndDate);

        // Validações e cálculos
        Task<bool> IsVehicleAvailableForRentalAsync(int vehicleId, DateTime startDate, DateTime endDate, int? excludeRentalId = null);
        Task<decimal> CalculateRentalCostAsync(int vehicleId, DateTime startDate, DateTime endDate);

        // Métodos de filtragem e paginação
        Task<IEnumerable<Rental>> GetFilteredRentalsAsync(
            string statusFilter = null,
            DateTime? startDateFilter = null,
            DateTime? endDateFilter = null,
            int pageNumber = 1,
            int pageSize = 10);

        Task<int> GetRentalsCountAsync(
            string statusFilter = null,
            DateTime? startDateFilter = null,
            DateTime? endDateFilter = null);
    }
}