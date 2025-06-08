using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Wheels_in_Csharp.Data;
using Wheels_in_Csharp.Models;
using Wheels_in_Csharp.Services.Interfaces;

namespace Wheels_in_Csharp.Services
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserService(
            ApplicationDbContext context,
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser> GetUserByIdAsync(string id)
        {
            return await _userManager.FindByIdAsync(id);
        }

        public async Task<ApplicationUser> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<IEnumerable<ApplicationUser>> GetAllUsersAsync()
        {
            return await _userManager.Users.ToListAsync();
        }

        public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Customer");
            }
            return result;
        }

        public async Task<IdentityResult> UpdateUserAsync(ApplicationUser user)
        {
            return await _userManager.UpdateAsync(user);
        }

        public async Task<IdentityResult> DeleteUserAsync(string userId)
        {
            var user = await GetUserByIdAsync(userId);
            return user == null ? null : await _userManager.DeleteAsync(user);
        }

        public async Task<IEnumerable<Rental>> GetUserRentalsAsync(string userId)
        {
            return await _context.Rentals
                .Include(r => r.RentedVehicle)
                .Where(r => r.CustomerId == userId)
                .OrderByDescending(r => r.StartTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<PaymentMethod>> GetUserPaymentMethodsAsync(string userId)
        {
            return await _context.PaymentMethods
                .Where(pm => pm.UserId == userId)
                .OrderByDescending(pm => pm.IsPrimary)
                .ToListAsync();
        }

        public async Task<PaymentMethod> AddUserPaymentMethodAsync(string userId, PaymentMethod paymentMethod)
        {
            paymentMethod.UserId = userId;
            _context.PaymentMethods.Add(paymentMethod);
            await _context.SaveChangesAsync();
            return paymentMethod;
        }

        public async Task<bool> RemoveUserPaymentMethodAsync(int paymentMethodId)
        {
            var paymentMethod = await _context.PaymentMethods.FindAsync(paymentMethodId);
            if (paymentMethod == null) return false;

            _context.PaymentMethods.Remove(paymentMethod);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsUserAdminAsync(string userId)
        {
            var user = await GetUserByIdAsync(userId);
            return user != null && await _userManager.IsInRoleAsync(user, "Admin");
        }
    }
}