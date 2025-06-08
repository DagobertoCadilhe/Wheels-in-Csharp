using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Wheels_in_Csharp.Models;

namespace Wheels_in_Csharp.Services.Interfaces
{
    public interface IUserService
    {
        Task<ApplicationUser> GetUserByIdAsync(string id);
        Task<ApplicationUser> GetUserByEmailAsync(string email);
        Task<IEnumerable<ApplicationUser>> GetAllUsersAsync();
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<IdentityResult> UpdateUserAsync(ApplicationUser user);
        Task<IdentityResult> DeleteUserAsync(string userId);
        Task<IEnumerable<Rental>> GetUserRentalsAsync(string userId);
        Task<IEnumerable<PaymentMethod>> GetUserPaymentMethodsAsync(string userId);
        Task<PaymentMethod> AddUserPaymentMethodAsync(string userId, PaymentMethod paymentMethod);
        Task<bool> RemoveUserPaymentMethodAsync(int paymentMethodId);
        Task<bool> IsUserAdminAsync(string userId);
    }
}